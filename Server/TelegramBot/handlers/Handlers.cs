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

            using var db = new MyDbContextFactory().CreateDbContext(null);

            if (Program.Cache.TryGetValue(chat_id, out object obj))
            {
                var docs = DbManipulations.GetStuffBySpec(db, msg);
                await OnStateChanged.OnMenuAction(docs, chat_id, bot, msg, obj as TelegramBotValuesModel);
            }
            else
            {
                var cat = DbManipulations.GetAllCategories(db);
                await OnStateChanged.OnMenuAction(cat, chat_id, bot, msg, new TelegramBotValuesModel());
            }
        }
    }
}
