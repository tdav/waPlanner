using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.Utils;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.RegularExpressions;
using waPlanner.TelegramBot.keyboards;

namespace waPlanner.TelegramBot.handlers
{
    public class Handlers
    {
        public static ITelegramBotClient Bot_;
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
            long chat_id = message.Chat.Id;
            string msg = message.Text;
            string message_for_user = "";

            using (var db = new MyDbContextFactory().CreateDbContext(null))
            {
                if (!Program.Cache.TryGetValue(chat_id, out var obj))
                {
                    Program.Cache[chat_id] = new TelegramBotValuesModel { State = PlannerStates.NONE, };
                }

                List<IdValue> menu = null;
                var cache = Program.Cache[chat_id] as TelegramBotValuesModel;

                if(msg == "⬅️Назад")
                {
                    switch (cache.State)
                    {
                        case PlannerStates.STUFF:
                            {
                                cache.State = PlannerStates.NONE;
                                break;
                            }
                        default:
                            {
                                cache.State = PlannerStates.CATEGORY;
                                break;
                            }  
                    }
                }
                if(msg == "Main")
                {
                    cache.State = PlannerStates.CATEGORY;
                }
                switch (cache.State)
                {
                    case PlannerStates.NONE:
                        {
                            await Bot_.SendTextMessageAsync(chat_id, "TEST", replyMarkup: ReplyKeyboards.MainMenu());
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
                            if (!DbManipulations.CheckCategory(db).Contains(msg) && msg != "⬅️Назад") return;
                            
                            cache.Category = DbManipulations.CheckCategory(db).Contains(msg)? msg : cache.Category;
                            menu = DbManipulations.GetStuffByCategory(db, cache.Category);
                            cache.State = msg != "⬅️Назад"? PlannerStates.CHOOSE_DATE: cache.State;
                            message_for_user = "Выберите специалиста";
                            break;
                        }
                    case PlannerStates.CHOOSE_DATE:
                        {
                            if (!DbManipulations.CheckStuffByCategory(db, cache.Category, msg) && msg != "⬅️Назад") return;

                            cache.Stuff =  msg != "⬅️Назад"? msg: cache.Stuff;
                            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            await Bot_.SendTextMessageAsync(chat_id, "Выберите удобное для вас число.", replyMarkup: back);
                            await Bot_.SendTextMessageAsync(chat_id, "Календарь", replyMarkup: CalendarKeyboards.SendCalendar(ref date));
                            return;
                        }
                    case PlannerStates.PHONE:
                        {
                            string phoneNumber = "";
                            if (message.Contact is not null)
                            {
                                phoneNumber = message.Contact.PhoneNumber;
                            }

                            if (!string.IsNullOrEmpty(msg))
                            {
                                string pattern = @"^\+\d{12}$";
                                Regex regex = new(pattern, RegexOptions.IgnorePatternWhitespace);
                                if (regex.IsMatch(msg))
                                    phoneNumber = msg;
                                else
                                {
                                    return;
                                }
                            }
                            cache.Phone = phoneNumber;
                            cache.State = PlannerStates.USERNAME;
                            await Bot_.SendTextMessageAsync(chat_id, "Введите ваше Ф.И.О", replyMarkup: back);
                            return;
                        }
                    case PlannerStates.USERNAME:
                        {
                            cache.UserName = msg;
                            await Bot_.SendTextMessageAsync(chat_id, "Ваша заявка принята, ждите звонка от оператора", replyMarkup: ReplyKeyboards.MainMenu());
                            cache.State = PlannerStates.NONE;
                            await DbManipulations.FinishProcessAsync(chat_id, cache, db);
                            await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                            return;
                        }
                    default:
                        break;
                }

                if(menu is not null)
                {
                    var buttons = ReplyKeyboards.SendKeyboards(menu);
                    ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                    await Bot_.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                    return;
                }
                await Bot_.SendTextMessageAsync(chat_id, "Выберайте по кнопкам");
            };
            
        }
        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            using var db = new MyDbContextFactory().CreateDbContext(null);
            await CalendarKeyboards.OnCalendarProcess(call, back, db);
            await TimeKeyboards.OnTimeProcess(call, Bot_, db);
        }
    }
}

