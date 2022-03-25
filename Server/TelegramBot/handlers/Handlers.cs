using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.handlers
{
    public class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(bot, update.Message!),
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(bot, exception, cancellationToken);
            }
        }
        private static async Task BotOnMessageReceived(ITelegramBotClient bot, Message message)
        {
            if (message.Type != MessageType.Text)
                return;
            long chat_id = message.Chat.Id;
            string msg = message.Text;

            using (var db = new MyDbContextFactory().CreateDbContext(null))
            {
                if (Program.Cache.TryGetValue(chat_id, out object obj))
                {
                    var value = obj as TelegramBotValuesModel;

                    switch (value.State)
                    {
                        case PlannerStates.CATEGORY:
                            value.State = PlannerStates.DOCTORS;
                            value.Category = msg;
                            Program.Cache[chat_id] = value;

                            var docs = db.tbUsers
                             .AsNoTracking()
                             .Include(i=>i.Category)
                             .Where(x => x.UserTypeId == 1 && x.Category.NameUz == msg)
                             .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                             .ToList();
                            ReplyKeyboardMarkup markup = new(Keyboards.SendKeyboards(docs)) { ResizeKeyboard = true };
                            await bot.SendTextMessageAsync(chat_id, "Выберите Врача", replyMarkup: markup);
                            break;
                        case PlannerStates.DOCTORS:

                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var cat = db.spCategories.AsNoTracking().Select(x => new IdValue { Id = x.Id, Name = x.NameUz }).ToList();
                    ReplyKeyboardMarkup markup = new(Keyboards.SendKeyboards(cat)) { ResizeKeyboard = true };
                    await bot.SendTextMessageAsync(chat_id, "Выберите категорию", replyMarkup: markup);
                    var value = new TelegramBotValuesModel();
                    value.State = PlannerStates.CATEGORY;
                    value.Category = msg;
                    Program.Cache[chat_id] = value;
                }

            }
        }
    }
}
