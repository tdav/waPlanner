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
using waPlanner.ModelViews;
using waPlanner.TelegramBot.Utils;
using System.Collections.Generic;
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
            string message_for_user = "";
            List<IdValue> menu = new();

            using var db = new MyDbContextFactory().CreateDbContext(null);

            if (Program.Cache.TryGetValue(chat_id, out object obj))
            {
                if (Program.Cache.TryGetValue(message.Chat.Id, out object obj))
                {
                var value = obj as TelegramBotValuesModel;

                switch (value.State)
                {
                    case PlannerStates.CATEGORY:
                        {
                            value.State = PlannerStates.DOCTORS;
                            value.Category = msg;
                            Program.Cache[chat_id] = value;
                            message_for_user = "Выберите врача";
                            menu = DbManipulations.GetStuffBySpec(db, msg);
                            break;
                        }
                    case PlannerStates.DOCTORS:
                        {
                            value.State = PlannerStates.CHOOSE_TIME;
                            va
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                var value = new TelegramBotValuesModel
                {
                    State = PlannerStates.CATEGORY,
                    Category = msg
                    
                };
                menu = DbManipulations.GetAllCategories(db);
                Program.Cache[chat_id] = value;
                message_for_user = "Выберите категорию";
            }
            ReplyKeyboardMarkup markup = new(Keyboards.SendKeyboards(menu)) { ResizeKeyboard = true };
            await bot.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);

        }
    }
}
}
