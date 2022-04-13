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

            using (var db = new MyDbContextFactory().CreateDbContext(null))
            {
                if (!Program.Cache.TryGetValue(chat_id, out var obj))
                {
                    Program.Cache[chat_id] = new TelegramBotValuesModel { State = PlannerStates.NONE, };
                }
                await OnStateChange(chat_id, db, message, back);
            }
        }
        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            using var db = new MyDbContextFactory().CreateDbContext(null);
            var cache = Program.Cache[call.Message.Chat.Id] as TelegramBotValuesModel;

            if (cache.State == PlannerStates.CHOOSE_TIME)
                await TimeKeyboards.OnTimeProcess(call, Bot_, db);
            else
                await CalendarKeyboards.OnCalendarProcess(call, back, db);
        }

        public static async Task OnStateChange(long chat_id, MyDbContext db, Message message, ReplyKeyboardMarkup back)
        {
            List<IdValue> menu = null;
            string message_for_user = "";
            string msg = message.Text;
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;

            if (msg == "⬅️Назад")
            {
                switch (cache.State)
                {
                    case PlannerStates.ORGANIZATION:
                        {
                            cache.State = PlannerStates.NONE;
                            break;
                        }
                    case PlannerStates.CATEGORY:
                        {
                            cache.State = PlannerStates.SPECIALIZATION;
                            break;
                        }
                    case PlannerStates.STUFF:
                        {
                            cache.State = PlannerStates.ORGANIZATION;
                            break;
                        }
                    case PlannerStates.CHOOSE_DATE:
                        {
                            cache.State = PlannerStates.CATEGORY;
                            break;
                        }
                    case PlannerStates.CHOOSE_TIME:
                        {
                            cache.State = PlannerStates.CHOOSE_DATE;
                            break;
                        }
                    case PlannerStates.PHONE:
                        {
                            cache.State = PlannerStates.CHOOSE_TIME;
                            await Bot_.SendTextMessageAsync(chat_id, msg, replyMarkup: back);
                            await Bot_.SendTextMessageAsync(chat_id, "Выберите удобное для вас время.", replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
                            return;
                        }
                    case PlannerStates.USERNAME:
                        {
                            cache.State = PlannerStates.PHONE;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            if (msg == "Сделать бронь📄")
                cache.State = PlannerStates.SPECIALIZATION;

            switch (cache.State)
            {
                case PlannerStates.NONE:
                    {
                        await Bot_.SendTextMessageAsync(chat_id, "Что пожелаете?☺️", replyMarkup: ReplyKeyboards.MainMenu());
                        return;
                    }
                case PlannerStates.SPECIALIZATION: 
                    {
                        menu = await DbManipulations.SendSpecializations(db);
                        message_for_user = "Выберите услугу";
                        cache.State = PlannerStates.ORGANIZATION;
                        break;
                    }
                case PlannerStates.ORGANIZATION:
                    {
                        cache.Specialization = msg != "⬅️Назад" ? msg : cache.Specialization;
                        message_for_user = "Выберите соответствующую организацию";
                        menu = await DbManipulations.SendOrganizations(db, cache.Specialization);
                        cache.Specialization = msg;
                        cache.State = PlannerStates.CATEGORY;
                        break;
                    }
                case PlannerStates.CATEGORY:
                    {
                        cache.Organization = msg != "⬅️Назад" ? msg : cache.Organization;
                        menu = await DbManipulations.SendCategoriesByOrgName(db, cache.Organization);
                        cache.State = PlannerStates.STUFF;
                        message_for_user = "Выберите категорию";
                        break;
                    }
                case PlannerStates.STUFF:
                    {
                        var check_category = await DbManipulations.CheckCategory(db);
                        if (!check_category.Contains(msg) && msg != "⬅️Назад") return;

                        cache.Category = msg != "⬅️Назад" ? msg : cache.Category;
                        menu = await DbManipulations.GetStaffByCategory(db, cache.Category);
                        cache.State = msg != "⬅️Назад" ? PlannerStates.CHOOSE_DATE : cache.State;
                        message_for_user = "Выберите специалиста";
                        break;
                    }
                case PlannerStates.CHOOSE_DATE:
                    {
                        if (!await DbManipulations.CheckStaffByCategory(db, cache.Category, msg) && msg != "⬅️Назад") return;

                        cache.Stuff = msg != "⬅️Назад" ? msg : cache.Stuff;
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
                                await ReplyKeyboards.RequestContactAsync(Bot_, chat_id);
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
                        await Bot_.SendTextMessageAsync(chat_id, "Ваша заявка принята, ждите звонка от оператора");
                        cache.State = PlannerStates.ADD_FAVORITES;
                        await Bot_.SendTextMessageAsync(chat_id, "Хотите выбранного специалиста в избранное?", replyMarkup: ReplyKeyboards.SendConfirmKeyboards());
                        await DbManipulations.FinishProcessAsync(chat_id, cache, db);
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                        break;
                    }
                case PlannerStates.ADD_FAVORITES:
                    {
                        cache.State = PlannerStates.NONE;
                        if (msg == "Нет❌")
                        {
                            break;
                        }
                        if(msg == "Да✅")
                        {
                            await DbManipulations.AddToFavorites(db, cache, chat_id);
                            await Bot_.SendTextMessageAsync(chat_id, $"Специалист <b>{cache.Stuff}</b> был добавлен в избранное", parseMode: ParseMode.Html);
                            await Bot_.SendTextMessageAsync(chat_id, "Что пожелаете?☺️", replyMarkup: ReplyKeyboards.MainMenu());
                            return;
                        }
                        break;
                    }
                default:
                    break;
            }

            if (menu is not null)
            {
                var buttons = ReplyKeyboards.SendKeyboards(menu);
                if (cache.State > 0) buttons.Add(new List<KeyboardButton> { new KeyboardButton("⬅️Назад") });
                ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                await Bot_.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                return;
            }
        }

        public async Task OnCommands(string command, long chat_id)
        {

        }
    }
}

