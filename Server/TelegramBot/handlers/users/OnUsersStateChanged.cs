using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.Database;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.keyboards;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.handlers.users
{
    public class OnUsersStateChanged
    {
        public static async Task OnStateChange(long chat_id, MyDbContext db, ITelegramBotClient Bot_, Message message, 
            List<IdValue> menu, ReplyKeyboardMarkup back, TelegramBotValuesModel cache)
        {
            string message_for_user = "";
            string msg = message.Text;

            if (msg == "⬅️Назад")
            {
                switch (cache.State)
                {
                    case PlannerStates.STUFF:
                        {
                            cache.State = PlannerStates.NONE;
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
                            cache.State = PlannerStates.CATEGORY;
                            break;
                        }
                }
            }

            switch (cache.State)
            {
                case PlannerStates.NONE:
                    {
                        List<IdValue> services = await DbManipulations.GetAllGlobalCats(db);
                        var servicesButtons = ReplyKeyboards.SendKeyboards(services);
                        ReplyKeyboardMarkup reply = new(servicesButtons) { ResizeKeyboard = true };
                        await Bot_.SendTextMessageAsync(chat_id, "Выберите услугу", replyMarkup: reply);
                        cache.State = PlannerStates.CATEGORY;
                        return;
                    }
                case PlannerStates.CATEGORY:
                    {
                        var check_services = await DbManipulations.CheckServices(db);
                        if (!check_services.Contains(msg) && msg != "⬅️Назад") return;
                        cache.GlobalCategory = msg != "⬅️Назад" ? msg : cache.GlobalCategory;
                        menu = await DbManipulations.GetCategoriesByType(db, cache.GlobalCategory);
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
                        menu = await DbManipulations.GetAllGlobalCats(db);
                        cache.State = PlannerStates.NONE;
                        message_for_user = "Выберите услугу";
                        await DbManipulations.FinishProcessAsync(chat_id, cache, db);
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
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
            await Bot_.SendTextMessageAsync(chat_id, "Выберайте по кнопкам");
        }
    }
}
