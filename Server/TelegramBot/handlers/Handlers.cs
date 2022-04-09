using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using waPlanner.ModelViews;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.TelegramBot.keyboards;
using waPlanner.TelegramBot.Utils;
using System.Text.RegularExpressions;

namespace waPlanner.TelegramBot.handlers
{
    public class Handlers
    {
        public static TelegramBotClient Bot_ = StartBot.Bot;
        private static ReplyKeyboardMarkup back = new(new[] { new KeyboardButton("⬅️Назад") }) { ResizeKeyboard = true };

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
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
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
        private static async Task BotOnMessageReceived(Message message)
        {
            long chat_id = message.Chat.Id;
            string msg = message.Text;

            using (var db = new MyDbContextFactory().CreateDbContext(null))
            {
                if (!Program.Cache.TryGetValue(chat_id, out var obj))
                {
                    Program.Cache[chat_id] = new TelegramBotValuesModel { State = PlannerStates.NONE, };
                }
                var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
                List<IdValue> menu = null;
                await users.OnUsersStateChanged.OnStateChange(chat_id, db, Bot_, message, back, cache);
            }
        }
        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            using var db = new MyDbContextFactory().CreateDbContext(null);
            await CalendarKeyboards.OnCalendarProcess(call, back, db);
            await TimeKeyboards.OnTimeProcess(call, Bot_, db);
        }
    }
}

