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
        
        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            using var db = new MyDbContextFactory().CreateDbContext(null);
            var cache = Program.Cache[call.Message.Chat.Id] as TelegramBotValuesModel;

            if (cache.State == PlannerStates.CHOOSE_TIME)
                await TimeKeyboards.OnTimeProcess(call, Bot_, db);
            else
                await CalendarKeyboards.OnCalendarProcess(call, back, db);
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
                var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
                await OnStateChanged(chat_id, db, message, cache);
            }
        }

        public static async Task OnStateChanged(long chat_id, MyDbContext db, Message message, TelegramBotValuesModel cache)
        {
            List<IdValue> menu = null;
            string message_for_user = "";
            string msg = message.Text;

            if (msg == Commands.back_)
            {
                switch (cache.State)
                {
                    case PlannerStates.SELECT_FAVORITES:
                        {
                            cache.State = PlannerStates.NONE;
                            break;
                        }
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

            if (msg == Commands.favorites_)
                cache.State = PlannerStates.FAVORITES;

            if (msg == Commands.reservation_)
                cache.State = PlannerStates.SPECIALIZATION;

            switch (cache.State)
            {
                case PlannerStates.NONE:
                    {
                        await Bot_.SendTextMessageAsync(chat_id, "Что пожелаете?☺️", replyMarkup: ReplyKeyboards.MainMenu());
                        return;
                    }
                case PlannerStates.FAVORITES:
                    {
                        menu = await DbManipulations.SendFavorites(db, chat_id);
                        if (menu.Count > 0)
                        {
                            cache.State = PlannerStates.SELECT_FAVORITES;
                            message_for_user = "Выберите специалиста";
                            break;
                        }
                        else
                        {
                            await Bot_.SendTextMessageAsync(chat_id, "Список пуст");
                            break;
                        }
                        
                    }
                case PlannerStates.SELECT_FAVORITES:
                    {
                        cache.Staff = msg != "⬅️Назад" ? msg : cache.Staff;
                        viStaffInfo staffInfo = await DbManipulations.GetStaffInfoByName(db, cache.Staff);
                        if (staffInfo is not null)
                        {
                            cache.Specialization = staffInfo.Specialization;
                            cache.Organization = staffInfo.Organization;
                            cache.Category = staffInfo.Category;
                            cache.Staff = staffInfo.Staff;
                            await CalendarKeyboards.SendCalendar(Bot_, chat_id, back);
                        }
                        break;
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

                        cache.Staff = msg != "⬅️Назад" ? msg : cache.Staff;
                        await CalendarKeyboards.SendCalendar(Bot_, chat_id, back);
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

                        if (msg == "Нет❌")
                        {

                        }
                        
                        if(msg == "Да✅")
                        {
                            await DbManipulations.AddToFavorites(db, cache, chat_id);
                            await Bot_.SendTextMessageAsync(chat_id, $"Специалист <b>{cache.Staff}</b> добавлен в избранное", parseMode: ParseMode.Html);
                        }
                        cache.State = PlannerStates.NONE;
                        await Bot_.SendTextMessageAsync(chat_id, "Что пожелаете?☺️", replyMarkup: ReplyKeyboards.MainMenu());
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
    }
}

