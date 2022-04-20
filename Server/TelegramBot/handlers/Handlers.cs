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
        public static TelegramBotClient bot = StartBot.Bot;
        public static string choose_lang = "Выберите язык\nTilni tanlang";


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
                UpdateType.MyChatMember => BotOnMyChatMemeber(update)
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
        
        public static async Task BotOnMyChatMemeber(Update update)
        {
            Console.WriteLine(update.MyChatMember.Chat.Id); 
        }

        public static async Task BotOnCallbackQueryReceived(CallbackQuery call)
        {
            using var db = new MyDbContextFactory().CreateDbContext(null);
            var cache = Program.Cache[call.Message.Chat.Id] as TelegramBotValuesModel;
            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs[cache.Lang]["back"]) }) { ResizeKeyboard = true };
            if (cache.State == PlannerStates.CHOOSE_TIME)
                await TimeKeyboards.OnTimeProcess(call, bot, db);
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
                    Program.Cache[chat_id] = new TelegramBotValuesModel { State = PlannerStates.NONE };
                }
                var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
                await OnCommands(cache, message.Text, chat_id, db);
                await OnStateChanged(chat_id, db, message, cache);
            }
        }





        public static async Task OnStateChanged(long chat_id, MyDbContext db, Message message, TelegramBotValuesModel cache)
        {
            List<IdValue> menu = null;
            string message_for_user = "";
            string msg = message.Text;
            
            switch (cache.State)
            {
                case PlannerStates.NONE:
                    {
                        await bot.SendTextMessageAsync(chat_id, choose_lang, replyMarkup: ReplyKeyboards.SendLanguages());
                        cache.State = PlannerStates.MAIN_MENU;
                        break;
                    }
                case PlannerStates.MAIN_MENU:
                    {
                        if (msg == "Русский🇷🇺")
                            cache.Lang = "ru";
                        else if (msg == "O'zbekcha🇺🇿")
                            cache.Lang = "uz";
                        else if (!Commands.back.Contains(msg)) return;

                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang));
                        break;
                    }
                case PlannerStates.FAVORITES:
                    {
                        menu = await DbManipulations.SendFavorites(db, chat_id);
                        if (menu is not null && menu.Count > 0)
                        {
                            cache.State = PlannerStates.SELECT_FAVORITES;
                            message_for_user = Program.langs[cache.Lang]["CHOOSE_SPECIALIST"];
                            break;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["EMPTY_LIST"]);
                            break;
                        }
                        
                    }
                case PlannerStates.SELECT_FAVORITES:
                    {
                        cache.Staff = msg != Program.langs[cache.Lang]["back"] ? msg : cache.Staff;
                        viStaffInfo staffInfo = await DbManipulations.GetStaffInfoByName(db, cache.Staff);
                        if (staffInfo is not null)
                        {
                            cache.Specialization = staffInfo.Specialization;
                            cache.Organization = staffInfo.Organization;
                            cache.Category = staffInfo.Category;
                            cache.Staff = staffInfo.Staff;
                            cache.State = PlannerStates.CHOOSE_DATE;
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                            await CalendarKeyboards.SendCalendar(bot, chat_id, back, cache.Lang);
                        }
                        break;
                    }

                case PlannerStates.SETTINGS:
                    {
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang));
                        break;
                    }
                case PlannerStates.CHANGE_LANG:
                    {
                        if (msg == "Русский🇷🇺")
                            cache.Lang = "ru";
                        else if (msg == "O'zbekcha🇺🇿")
                            cache.Lang = "uz";
                        else return;

                        cache.State = PlannerStates.SETTINGS;
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang));
                        break;
                    }
                case PlannerStates.PREPARE_CHANGE_NAME:
                    {
                        cache.State = PlannerStates.CHANGE_NAME;
                        break;
                    }
                case PlannerStates.CHANGE_NAME:
                    {
                        await DbManipulations.UpdateUserName(db, chat_id, msg);
                        cache.State = PlannerStates.SETTINGS;
                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["CONFIRMED"] + msg);
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang));
                        break;
                    }
                case PlannerStates.CHANGE_PHONE:
                    {
                        if (Utils.Utils.CheckPhone(msg))
                        {
                            await DbManipulations.UpdateUserPhone(db, chat_id, msg);
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["CONFIRMED"]);
                            cache.State = PlannerStates.SETTINGS;
                            await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang));
                        }
                        else
                        {
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["ON_CHANGE_PHONE"], parseMode: ParseMode.Html, replyMarkup: back);
                            return;
                        }     
                        break;
                    }

                case PlannerStates.SPECIALIZATION: 
                    {
                        menu = await DbManipulations.SendSpecializations(db);
                        message_for_user = Program.langs[cache.Lang]["CHOOSE_SERVICE"];
                        cache.State = PlannerStates.ORGANIZATION;
                        break;
                    }
                case PlannerStates.ORGANIZATION:
                    {
                        cache.Specialization = msg != Program.langs[cache.Lang]["back"] ? msg : cache.Specialization;
                        message_for_user = Program.langs[cache.Lang]["CHOOSE_ORG"];
                        menu = await DbManipulations.SendOrganizations(db, cache.Specialization);
                        cache.Specialization = msg;
                        cache.State = PlannerStates.CATEGORY;
                        break;
                    }
                case PlannerStates.CATEGORY:
                    {
                        cache.Organization = !Commands.back.Contains(msg) ? msg : cache.Organization;
                        menu = await DbManipulations.SendCategoriesByOrgName(db, cache.Organization);
                        cache.State = PlannerStates.STUFF;
                        message_for_user = Program.langs[cache.Lang]["CHOOSE_CAT"];
                        break;
                    }
                case PlannerStates.STUFF:
                    {
                        var check_category = await DbManipulations.CheckCategory(db);
                        if (!check_category.Contains(msg) && msg != Program.langs[cache.Lang]["back"]) return;

                        cache.Category = msg != Program.langs[cache.Lang]["back"] ? msg : cache.Category;
                        menu = await DbManipulations.GetStaffByCategory(db, cache.Category);
                        cache.State = msg != Program.langs[cache.Lang]["back"] ? PlannerStates.CHOOSE_DATE : cache.State;
                        message_for_user = Program.langs[cache.Lang]["CHOOSE_SPECIALIST"];
                        break;
                    }
                case PlannerStates.CHOOSE_DATE:
                    {
                        if (!await DbManipulations.CheckStaffByCategory(db, cache.Category, msg) && !Commands.back.Contains(msg)) return;

                        ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                        cache.Staff = msg != Program.langs[cache.Lang]["back"] ? msg : cache.Staff;
                        await CalendarKeyboards.SendCalendar(bot, chat_id, back, cache.Lang);
                        return;
                    }
                case PlannerStates.PHONE:
                    {
                        string phoneNumber = "";
                        if (message.Contact is not null)
                        {
                            phoneNumber = message.Contact.PhoneNumber;
                        }

                        if (Utils.Utils.CheckPhone(msg))
                              phoneNumber = msg;
                        else
                        {
                            await ReplyKeyboards.RequestContactAsync(bot, chat_id, cache.Lang);
                            return;
                        }
                        cache.Phone = phoneNumber;
                        cache.State = PlannerStates.USERNAME;
                        ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["SEND_FULL_NAME"], replyMarkup: back);
                        return;
                    }
                case PlannerStates.USERNAME:
                    {
                        cache.UserName = msg;
                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["BID"]);
                        await DbManipulations.FinishProcessAsync(chat_id, cache, db);
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                        await Utils.Utils.SendOrder(cache, bot, db);
                        if (!await DbManipulations.CheckFavorites(db, cache.Staff, chat_id))
                        {
                            cache.State = PlannerStates.ADD_FAVORITES;
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["ADD_FAVORITES"], replyMarkup: ReplyKeyboards.SendConfirmKeyboards(cache.Lang));
                            break;
                        }
                        cache.State = PlannerStates.NONE;
                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang));
                        break;
                    }
                case PlannerStates.ADD_FAVORITES: 
                    {

                        if (msg == Program.langs[cache.Lang]["NO"])
                        {

                        }
                        
                        if(msg == Program.langs[cache.Lang]["YES"])
                        {
                            await DbManipulations.AddToFavorites(db, cache, chat_id);
                            await bot.SendTextMessageAsync(chat_id, 
                                $"{Program.langs[cache.Lang]["SPECIALIST"]} <b>{cache.Staff}</b> {Program.langs[cache.Lang]["INTO_FAVORITES"]}", 
                                parseMode: ParseMode.Html);
                        }
                        cache.State = PlannerStates.NONE;
                        await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang));
                        break;
                    }
                default:
                    break;
            }
            
            if (menu is not null)
            {
                var buttons = ReplyKeyboards.SendKeyboards(menu);
                if (cache.State > 0) buttons.Add(new List<KeyboardButton> { new KeyboardButton(Program.langs[cache.Lang]["back"]) });
                ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                return;
            }
        }
        public static async Task OnCommands(TelegramBotValuesModel cache, string msg, long chat_id, MyDbContext db)
        {
            if (Commands.back.Contains(msg))
            {
                switch (cache.State)
                {
                    case PlannerStates.SETTINGS:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
                            break;
                        }
                    case PlannerStates.CHANGE_LANG:
                        {
                            cache.State = PlannerStates.SETTINGS;
                            break;
                        }
                    case PlannerStates.CHANGE_NAME:
                        {
                            cache.State = PlannerStates.SETTINGS;
                            break;
                        }
                    case PlannerStates.CHANGE_PHONE:
                        {
                            cache.State = PlannerStates.SETTINGS;
                            break;
                        }

                    case PlannerStates.SELECT_FAVORITES:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
                            break;
                        }
                    case PlannerStates.ORGANIZATION:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
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
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                            cache.State = PlannerStates.CHOOSE_TIME;
                            await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: back);
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["CUZY_TIME"], replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
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

            if (Commands.favorites.Contains(msg))
                cache.State = PlannerStates.FAVORITES;

            if (Commands.book.Contains(msg))
                cache.State = PlannerStates.SPECIALIZATION;

            if (Commands.settings.Contains(msg))
                cache.State = PlannerStates.SETTINGS;

            if (Commands.change_lang.Contains(msg))
            {
                cache.State = PlannerStates.CHANGE_LANG;
                await bot.SendTextMessageAsync(chat_id, choose_lang, replyMarkup: ReplyKeyboards.SendLanguages());
            }

            if (Commands.change_name.Contains(msg))
            {
                if (await DbManipulations.GetUserId(chat_id, db) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(Program.langs.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: back);
                cache.State = PlannerStates.PREPARE_CHANGE_NAME;
                return;
            }

            if (Commands.change_phone.Contains(msg))
            {
                if (await DbManipulations.GetUserId(chat_id, db) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                cache.State = PlannerStates.CHANGE_PHONE;
                return;
            }    
        }
    }
}

