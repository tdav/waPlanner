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
        public static ITelegramBotClient Bot_;

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
            Bot_ = bot;
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
            if (message.Type != MessageType.Text)
                return;

            long chat_id = message.Chat.Id;
            string msg = message.Text;
            string message_for_user = "";

            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton[] { "⬅️Назад" } }) { ResizeKeyboard = true };

            using var db = new MyDbContextFactory().CreateDbContext(null);
            if (!Program.Cache.TryGetValue(chat_id, out var obj))
            {
                Program.Cache[chat_id] = new TelegramBotValuesModel { State = PlannerStates.NONE, };
            }

            List<IdValue> menu = null;
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            switch (cache.State)
            {
                case PlannerStates.NONE: 
                    {
                        cache.State = PlannerStates.CATEGORY;
                        await Bot_.SendTextMessageAsync(chat_id, "TEST", replyMarkup: keyboards.MainMenuKeyboards.MainMenu());
                        return;
                    }
                case PlannerStates.CATEGORY:
                    {
                        menu = DbManipulations.GetAllCategories(db);
                        cache.State = PlannerStates.STUFF;
                        message_for_user = "Выберите категорию";
                        break;
                    }
                case PlannerStates.STUFF:
                    {
                        cache.Category = msg;
                        menu = DbManipulations.GetStuffByCategory(db, msg);
                        cache.State = PlannerStates.CHOOSE_DATE;
                        message_for_user = "Выберите специалиста";
                        break;
                    }
                case PlannerStates.CHOOSE_DATE:
                    {
                        if (!DbManipulations.CheckStuffByCategory(db, cache.Category, msg)) return;

                        cache.Stuff = msg;
                        int month = DateTime.Now.Month;
                        var date = new DateTime(DateTime.Now.Year, month, 1);
                        await Bot_.SendTextMessageAsync(chat_id, "Выберите удобное для вас число.", replyMarkup: back);
                        await Bot_.SendTextMessageAsync(chat_id, "Календарь", replyMarkup: keyboards.CalendarKeyboards.SendCalendar(date, month));
                        return;
                    }
                default:
                    break;
            }

            var buttons = keyboards.ReplyKeyboards.SendKeyboards(menu);
            if (cache.State == PlannerStates.CHOOSE_DATE) buttons.Add(new List<KeyboardButton> { new KeyboardButton("⬅️Назад") });
            ReplyKeyboardMarkup markup = new (buttons) { ResizeKeyboard = true };
            await Bot_.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
        }
        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            ReplyKeyboardMarkup back = new (new[] { new KeyboardButton("⬅️Назад") }) { ResizeKeyboard = true };
            await keyboards.CalendarKeyboards.OnCalendarProcess(call, back);

            Console.WriteLine(call.Data);
        }
    }
}

