using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;
using waPlanner.Services;
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
        private readonly Vars vars;
        private readonly IMemoryCache cache;
        private ITelegramBotClient bot;
        private readonly string choose_lang = "Выберите язык\nTilni tanlang";

        public BotService(IServiceProvider provider, IOptions<LangsModel> options, IOptions<Vars> voptions, IMemoryCache caches)
        {
            this.provider = provider;
            this.cache = caches;
            this.lang = options.Value;
            this.vars = voptions.Value;
        }

        public static Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        //public async Task HandleAsync1(ITelegramBotClient bot, Update[] updates, CancellationToken token)
        //{
        //    var qrcode = provider.GetService<IGenerateQrCode>();
        //    var ba = qrcode.Run("url");

        //    using (var ms = new MemoryStream(ba))
        //    {
        //        var aa = new InputOnlineFile(ms);
        //        await bot.SendPhotoAsync()
        //    }

        //}


        public async Task HandleAsync(ITelegramBotClient bot, Update[] updates, CancellationToken token)
        {
            this.bot = bot;

            try
            {
                foreach (var update in updates)
                {
                    if (update != null)
                    {
                        long chat_id = update.Message is not null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
                        using (var scope = provider.CreateScope())
                        {

                            if (!cache.TryGetValue(chat_id, out TelegramBotValuesModel telg_obj))
                            {
                                telg_obj = new TelegramBotValuesModel() { State = PlannerStates.NONE };
                                cache.Set(chat_id, telg_obj, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(vars.CacheTimeOut)));
                            }

                            var db = scope.ServiceProvider.GetRequiredService<IDbManipulations>();

                            if (string.IsNullOrEmpty(telg_obj.Lang))
                            {
                                var st = await db.GetUserLang(chat_id);
                                if (st is not null)
                                {
                                    telg_obj.Lang = st.Lang;
                                    telg_obj.State = PlannerStates.MAIN_MENU;
                                }
                            }

                            switch (update.Type)
                            {
                                case UpdateType.Message:
                                    {
                                        await BotOnMessageReceivedAsync(update.Message, db, telg_obj);
                                        break;
                                    }
                                case UpdateType.CallbackQuery:
                                    {
                                        await BotOnCallbackQueryReceivedAsync(update.CallbackQuery, db, telg_obj);
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task BotOnCallbackQueryReceivedAsync(CallbackQuery call, IDbManipulations db, TelegramBotValuesModel cache)
        {
            if (cache.State == PlannerStates.CHOOSE_TIME)
                await TimeKeyboards.OnTimeProcess(call, bot, db, lang, cache);
            else
                await CalendarKeyboards.OnCalendarProcess(call, db, bot, lang, cache);
        }

        private async Task BotOnMessageReceivedAsync(Message message, IDbManipulations db, TelegramBotValuesModel cache)
        {
            long chat_id = message.Chat.Id;
            Console.WriteLine(chat_id);

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
                        try
                        {
                            if (msg == "Русский🇷🇺")
                                cache.Lang = "ru";
                            else if (msg == "O'zbekcha🇺🇿")
                                cache.Lang = "uz";

                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                            break;
                        }
                        catch (ArgumentNullException)
                        {
                            await bot.SendTextMessageAsync(chat_id, choose_lang, replyMarkup: ReplyKeyboards.SendLanguages());
                            return;
                        }
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
                            cache.State = PlannerStates.MAIN_MENU;
                            return;
                        }

                    }
                case PlannerStates.SELECT_FAVORITES:
                    {
                        string staff_name = msg.Substring(msg.LastIndexOf(')') + 2);
                        cache.Staff = msg != lang[cache.Lang]["back"] ? staff_name : cache.Staff;
                        viStaffInfo staffInfo = await DbManipulations.GetStaffInfoByName(cache.Staff);
                        if (staffInfo is not null)
                        {
                            cache.Specialization = staffInfo.Specialization;
                            cache.Organization = staffInfo.Organization;
                            cache.Category = staffInfo.Category;
                            cache.Staff = staffInfo.Staff;
                            cache.State = PlannerStates.CHOOSE_DATE;
                            await CalendarKeyboards.SendCalendar(bot, chat_id, cache.Lang, lang);
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

                        await DbManipulations.UpdateUserLang(chat_id, cache.Lang);
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
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ON_CHANGE_PHONE"], parseMode: ParseMode.Html,
                                replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                            return;
                        }
                        break;
                    }

                case PlannerStates.ON_PREPARE_FEEDBACK:
                    {
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ON_FEEDBACK"], replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                        cache.State = PlannerStates.FEEDBACK;
                        break;
                    }
                case PlannerStates.FEEDBACK:
                    {
                        if (msg != lang[cache.Lang]["back"])
                        {
                            string link = $"tg://user?id={chat_id}";
                            string value = $"{message.From.FirstName} {message.From.LastName}";
                            var feedback = $"Отзыв от <a href='{link}'>{value}</a> ID:<b>{chat_id}</b>";
                            await bot.SendTextMessageAsync(Config.DEV_GROUP_ID, $"{feedback}: {msg}", parseMode: ParseMode.Html, disableWebPagePreview: true);
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SUCCESS_FEEDBACK"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                            cache.State = PlannerStates.MAIN_MENU;
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
                        cache.Organization = msg != lang[cache.Lang]["back"] ? msg : cache.Organization;
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
                        if (!await DbManipulations.CheckStaffByCategory(cache.Category, msg) && msg != lang[cache.Lang]["back"]) return;

                        cache.Staff = msg != lang[cache.Lang]["back"] ? msg : cache.Staff;
                        await CalendarKeyboards.SendCalendar(bot, chat_id, cache.Lang, lang);
                        return;
                    }
                case PlannerStates.PHONE:
                    {
                        string phoneNumber;
                        if (message.Contact is not null)
                        {
                            phoneNumber = message.Contact.PhoneNumber;
                        }

                        else if (Utils.Utils.CheckPhone(msg))
                            phoneNumber = msg;
                        else
                        {
                            await ReplyKeyboards.RequestContactAsync(bot, chat_id, cache.Lang, lang);
                            return;
                        }
                        cache.Phone = phoneNumber;
                        cache.State = PlannerStates.USERNAME;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SEND_FULL_NAME"], replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
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

                        else if (msg == lang[cache.Lang]["YES"])
                        {
                            await DbManipulations.AddToFavorites(cache, chat_id);
                            await bot.SendTextMessageAsync(chat_id,
                                $"{lang[cache.Lang]["SPECIALIST"]} <b>{cache.Staff}</b> {lang[cache.Lang]["INTO_FAVORITES"]}",
                                parseMode: ParseMode.Html);
                        }
                        else return;
                        cache.State = PlannerStates.NONE;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                        break;
                    }
                default:
                    break;
            }

            if (menu is not null)
            {
                var buttons = ReplyKeyboards.SendMenuKeyboards(menu);
                if (cache.State > 0) buttons.Add(new List<KeyboardButton> { new KeyboardButton(lang[cache.Lang]["back"]) });
                ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                return;
            }
        }
        private async Task OnCommands(TelegramBotValuesModel cache, string msg, long chat_id, IDbManipulations db)
        {
            if (msg == lang[cache.Lang]["back"])
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

                    case PlannerStates.FEEDBACK:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
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
                            cache.State = PlannerStates.CHOOSE_TIME;
                            await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
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

            else if (msg == lang[cache.Lang]["favorites"])
                cache.State = PlannerStates.FAVORITES;

            else if (msg == lang[cache.Lang]["book"])
                cache.State = PlannerStates.SPECIALIZATION;

            else if (msg == lang[cache.Lang]["settings"])
                cache.State = PlannerStates.SETTINGS;

            else if (msg == lang[cache.Lang]["change_lang"])
            {
                cache.State = PlannerStates.CHANGE_LANG;
                await bot.SendTextMessageAsync(chat_id, choose_lang, replyMarkup: ReplyKeyboards.SendLanguages());
            }

            else if (msg == lang[cache.Lang]["change_name"])
            {
                if (await db.GetUserId(chat_id) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                await bot.SendTextMessageAsync(chat_id, msg, replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                cache.State = PlannerStates.PREPARE_CHANGE_NAME;
                return;
            }

            else if (msg == lang[cache.Lang]["change_phone"])
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

            else if (msg == lang[cache.Lang]["feedback"])
            {
                cache.State = PlannerStates.ON_PREPARE_FEEDBACK;
                return;
            }

            else if (msg == lang[cache.Lang]["about_us"])
            {
                await bot.SendTextMessageAsync(chat_id, "MALUMOT");
            }

            else if (msg == lang[cache.Lang]["contacts"])
            {
                await bot.SendTextMessageAsync(chat_id, "MALUMOT");
            }

            else if (msg == lang[cache.Lang]["statistic"])
            {
                await bot.SendTextMessageAsync(chat_id, await Utils.Utils.SendStatistic(db, cache, lang), parseMode: ParseMode.Html);
            }
        }

    }
}