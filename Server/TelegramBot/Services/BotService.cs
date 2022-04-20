using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.handlers;
using waPlanner.TelegramBot.keyboards;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.Services
{
    public interface IBotService
    {
        Task HandleAsync(ITelegramBotClient bot, Update[] updates, CancellationToken token);
    }

    public class BotService : IBotService
    {
        private readonly IServiceProvider provider;
        private readonly LangsModel lang;
        private ITelegramBotClient bot;

        public BotService(IServiceProvider provider, IOptions<LangsModel> options)
        {
            this.provider = provider;
            this.lang = options.Value;
        }

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
        public async Task HandleAsync(ITelegramBotClient bot, Update[] updates, CancellationToken token)
        {
            this.bot = bot;


            try
            {
                foreach (var update in updates)
                {
                    if (update != null)
                    {
                        using (var scope = provider.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<IDbManipulations>();

                            switch (update.Type)
                            {
                                case UpdateType.Message:
                                    {
                                        if (!Program.Cache.TryGetValue(update.Message.Chat.Id, out var obj))
                                        {
                                            Program.Cache[update.Message.Chat.Id] = new TelegramBotValuesModel() { State = PlannerStates.NONE };
                                        }
                                        var cache = Program.Cache[update.Message.Chat.Id] as TelegramBotValuesModel;
                                        await BotOnMessageReceivedAsync(update.Message, db, cache);
                                        break;
                                    }
                                case UpdateType.CallbackQuery:
                                    {
                                        await BotOnCallbackQueryReceivedAsync(update.CallbackQuery, db);
                                        break;
                                    }
                                    
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(bot, ex, token);
            }
        }


        private async Task BotOnCallbackQueryReceivedAsync(CallbackQuery call, IDbManipulations db)
        {
            var cache = Program.Cache[call.Message.Chat.Id] as TelegramBotValuesModel;
            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang[cache.Lang]["back"]) }) { ResizeKeyboard = true };
            if (cache.State == PlannerStates.CHOOSE_TIME)
                await TimeKeyboards.OnTimeProcess(call, bot, db, lang);
            else
                await CalendarKeyboards.OnCalendarProcess(call, back, db, bot, lang);
        }

        private async Task BotOnMessageReceivedAsync(Message message, IDbManipulations db, TelegramBotValuesModel cache)
        {
            long chat_id = message.Chat.Id;
            if (cache.Lang is not null)
                await OnCommands(cache, message.Text, chat_id, db);
            await OnStateChanged(chat_id, message, cache, db);
        }

        private async Task OnStateChanged(long chat_id, Message message, TelegramBotValuesModel cache, IDbManipulations DbManipulations)
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

                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                        break;
                    }
                case PlannerStates.FAVORITES:
                    {
                        menu = await DbManipulations.SendFavorites(chat_id);
                        if (menu is not null && menu.Count > 0)
                        {
                            cache.State = PlannerStates.SELECT_FAVORITES;
                            message_for_user = lang[cache.Lang]["CHOOSE_SPECIALIST"];
                            break;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["EMPTY_LIST"]);
                            break;
                        }

                    }
                case PlannerStates.SELECT_FAVORITES:
                    {
                        cache.Staff = msg != lang[cache.Lang]["back"] ? msg : cache.Staff;
                        viStaffInfo staffInfo = await DbManipulations.GetStaffInfoByName(cache.Staff);
                        if (staffInfo is not null)
                        {
                            cache.Specialization = staffInfo.Specialization;
                            cache.Organization = staffInfo.Organization;
                            cache.Category = staffInfo.Category;
                            cache.Staff = staffInfo.Staff;
                            cache.State = PlannerStates.CHOOSE_DATE;
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                            await CalendarKeyboards.SendCalendar(bot, chat_id, back, cache.Lang, lang);
                        }
                        break;
                    }

                case PlannerStates.SETTINGS:
                    {
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
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
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
                        break;
                    }
                case PlannerStates.PREPARE_CHANGE_NAME:
                    {
                        cache.State = PlannerStates.CHANGE_NAME;
                        break;
                    }
                case PlannerStates.CHANGE_NAME:
                    {
                        await DbManipulations.UpdateUserName(chat_id, msg);
                        cache.State = PlannerStates.SETTINGS;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CONFIRMED"] + msg);
                        await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
                        break;
                    }
                case PlannerStates.CHANGE_PHONE:
                    {
                        if (Utils.Utils.CheckPhone(msg))
                        {
                            await DbManipulations.UpdateUserPhone(chat_id, msg);
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CONFIRMED"]);
                            cache.State = PlannerStates.SETTINGS;
                            await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
                        }
                        else
                        {
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ON_CHANGE_PHONE"], parseMode: ParseMode.Html, replyMarkup: back);
                            return;
                        }
                        break;
                    }

                case PlannerStates.SPECIALIZATION:
                    {
                        menu = await DbManipulations.SendSpecializations();
                        message_for_user = lang[cache.Lang]["CHOOSE_SERVICE"];
                        cache.State = PlannerStates.ORGANIZATION;
                        break;
                    }
                case PlannerStates.ORGANIZATION:
                    {
                        cache.Specialization = msg != lang[cache.Lang]["back"] ? msg : cache.Specialization;
                        message_for_user = lang[cache.Lang]["CHOOSE_ORG"];
                        menu = await DbManipulations.SendOrganizations(cache.Specialization);
                        cache.Specialization = msg;
                        cache.State = PlannerStates.CATEGORY;
                        break;
                    }
                case PlannerStates.CATEGORY:
                    {
                        cache.Organization = !Commands.back.Contains(msg) ? msg : cache.Organization;
                        menu = await DbManipulations.SendCategoriesByOrgName(cache.Organization);
                        cache.State = PlannerStates.STUFF;
                        message_for_user = lang[cache.Lang]["CHOOSE_CAT"];
                        break;
                    }
                case PlannerStates.STUFF:
                    {
                        var check_category = await DbManipulations.CheckCategory();
                        if (!check_category.Contains(msg) && msg != lang[cache.Lang]["back"]) return;

                        cache.Category = msg != lang[cache.Lang]["back"] ? msg : cache.Category;
                        menu = await DbManipulations.GetStaffByCategory(cache.Category);
                        cache.State = msg != lang[cache.Lang]["back"] ? PlannerStates.CHOOSE_DATE : cache.State;
                        message_for_user = lang[cache.Lang]["CHOOSE_SPECIALIST"];
                        break;
                    }
                case PlannerStates.CHOOSE_DATE:
                    {
                        if (!await DbManipulations.CheckStaffByCategory(cache.Category, msg) && !Commands.back.Contains(msg)) return;

                        ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                        cache.Staff = msg != lang[cache.Lang]["back"] ? msg : cache.Staff;
                        await CalendarKeyboards.SendCalendar(bot, chat_id, back, cache.Lang, lang);
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
                            await ReplyKeyboards.RequestContactAsync(bot, chat_id, cache.Lang, lang);
                            return;
                        }
                        cache.Phone = phoneNumber;
                        cache.State = PlannerStates.USERNAME;
                        ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang[cache.Lang]["back"]) }) { ResizeKeyboard = true };
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SEND_FULL_NAME"], replyMarkup: back);
                        return;
                    }
                case PlannerStates.USERNAME:
                    {
                        cache.UserName = msg;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["BID"]);
                        await DbManipulations.FinishProcessAsync(chat_id, cache);
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache);
                        await Utils.Utils.SendOrder(cache, bot, DbManipulations, chat_id);
                        if (!await DbManipulations.CheckFavorites(cache.Staff, chat_id))
                        {
                            cache.State = PlannerStates.ADD_FAVORITES;
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ADD_FAVORITES"], replyMarkup: ReplyKeyboards.SendConfirmKeyboards(cache.Lang, lang));
                            break;
                        }
                        cache.State = PlannerStates.NONE;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                        break;
                    }
                case PlannerStates.ADD_FAVORITES:
                    {

                        if (msg == lang[cache.Lang]["NO"])
                        {

                        }

                        if (msg == lang[cache.Lang]["YES"])
                        {
                            await DbManipulations.AddToFavorites(cache, chat_id);
                            await bot.SendTextMessageAsync(chat_id,
                                $"{lang[cache.Lang]["SPECIALIST"]} <b>{cache.Staff}</b> {lang[cache.Lang]["INTO_FAVORITES"]}",
                                parseMode: ParseMode.Html);
                        }
                        cache.State = PlannerStates.NONE;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                        break;
                    }
                default:
                    break;
            }

            if (menu is not null)
            {
                var buttons = ReplyKeyboards.SendKeyboards(menu);
                if (cache.State > 0) buttons.Add(new List<KeyboardButton> { new KeyboardButton(lang[cache.Lang]["back"]) });
                ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                return;
            }
        }
        private async Task OnCommands(TelegramBotValuesModel cache, string msg, long chat_id, IDbManipulations db)
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
                            ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                            cache.State = PlannerStates.CHOOSE_TIME;
                            await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: back);
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CUZY_TIME"], replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
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
                if (await db.GetUserId(chat_id) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                ReplyKeyboardMarkup back = new(new[] { new KeyboardButton(lang.Get(cache.Lang, "back")) }) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: back);
                cache.State = PlannerStates.PREPARE_CHANGE_NAME;
                return;
            }

            if (Commands.change_phone.Contains(msg))
            {
                if (await db.GetUserId(chat_id) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                cache.State = PlannerStates.CHANGE_PHONE;
                return;
            }
        }

    }
}