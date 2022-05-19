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

        public BotService(IServiceProvider provider, IOptions<LangsModel> options, IOptions<Vars> voptions, IMemoryCache cache)
        {
            this.provider = provider;
            this.cache = cache;
            lang = options.Value;
            vars = voptions.Value;
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

        public async Task HandleAsync(ITelegramBotClient bot, Update[] updates, CancellationToken token)
        {
            this.bot = bot;

            try
            {
                foreach (var update in updates)
                {
                    if (update is null) return;

                    if (update.Type == UpdateType.Message || update.Type == UpdateType.CallbackQuery)
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
                                        if (update.Message.Chat.Type == ChatType.Private)
                                        {
                                            if (update.Message.Type != MessageType.Contact && update.Message.Text.Length > 6 && update.Message.Text[0..6] == "/start")
                                            {

                                                await db.AddNewUserChat(chat_id);
                                                if (update.Message.Text[7] == 'o')
                                                {
                                                    int.TryParse(update.Message.Text[7..].Replace("o", ""), out int org_link);
                                                    if (!await db.CheckOrgFavorites(chat_id, org_link))
                                                    {
                                                        await db.AddOrgFavorites(org_link, chat_id);
                                                    }

                                                }

                                                else
                                                {
                                                    long.TryParse(update.Message.Text[6..].Replace(" ", ""), out long deep_link);
                                                    if (!await db.CheckFavorites(telg_obj.Staff, chat_id, deep_link))
                                                    {
                                                        await db.AddToFavorites(telg_obj, chat_id, deep_link);
                                                    }
                                                }
                                                await bot.SendTextMessageAsync(chat_id, "Добавился в избранное\nSevimlilarga qo'shildi");
                                            }
                                            await BotOnMessageReceivedAsync(update.Message, db, telg_obj);
                                        }
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

        private async Task GenerateQr(long chat_id, TelegramBotValuesModel cache, IDbManipulations db)
        {
            var my_bot = await bot.GetMeAsync();
            int user_role = await db.GetStaffRole(chat_id);
            string url = $"https://t.me/{my_bot.Username}?start={chat_id}";

            if (user_role != 0 && user_role == (int)UserRoles.ADMIN)
            {
                var admin_org = await db.GetAdminOrganization(chat_id);
                url = $"https://t.me/{my_bot.Username}?start=o{admin_org}";
            }

            using (var scope = provider.CreateScope())
            {
                var qrs = scope.ServiceProvider.GetService<IGenerateQrCodeService>();
                var data = qrs.Run(url);
                InputOnlineFile photo = new InputOnlineFile(new MemoryStream(data));

                await bot.SendPhotoAsync(chat_id, photo);
                cache.State = PlannerStates.MAIN_MENU;
            }
        }

        private async Task BotOnCallbackQueryReceivedAsync(CallbackQuery call, IDbManipulations db, TelegramBotValuesModel cache)
        {
            long chat_id = call.Message.Chat.Id;
            switch (cache.State)
            {
                case PlannerStates.CHOOSE_TIME:
                    {
                        await TimeKeyboards.OnTimeProcess(call, bot, db, lang, cache);
                        break;
                    }
                case PlannerStates.ANALYSIS:
                    {
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SEARCHING_FILE"]);
                        DateTime date = DateTime.Parse(call.Data);
                        ////await Utils.Utils.SendAnalysisResult(chat_id, cache, db, lang, date, bot);
                        //Thread thread = new(async () => await Utils.Utils.SendAnalysisResult(chat_id, cache, db, lang, date, bot));
                        //thread.Start();
                        await Utils.Utils.SendAnalysisResult(chat_id, cache, db, lang, date, bot);
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["COMPLETED_ANALYS"]);
                        break;
                    }
                case PlannerStates.CHOOSE_DATE:
                    {
                        await CalendarKeyboards.OnCalendarProcess(call, db, bot, lang, cache);
                        break;
                    }
                default:
                    break;
            }
        }

        private async Task BotOnMessageReceivedAsync(Message message, IDbManipulations db, TelegramBotValuesModel cache)
        {
            long chat_id = message.Chat.Id;
            if (Utils.Utils.CheckUserCommand(message.Text, cache, lang))
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

                case PlannerStates.PREPARE_CHECK_PHONE:
                    {
                        cache.State = PlannerStates.CHECK_PHONE;
                        break;
                    }
                case PlannerStates.CHECK_PHONE:
                    {
                        if (await DbManipulations.CheckNumber(msg) && msg != lang[cache.Lang]["back"])
                        {
                            cache.State = PlannerStates.CHECK_PASSWORD;
                            cache.Phone = msg;
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["PASSWORD"]);
                            break;
                        }
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["WRONG_INFO"]);
                        return;
                    }
                case PlannerStates.CHECK_PASSWORD:
                    {
                        if (await DbManipulations.CheckPassword(msg, chat_id, cache.Phone) && msg != lang[cache.Lang]["back"])
                        {
                            await GenerateQr(chat_id, cache, DbManipulations);
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                            break;
                        }

                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["WRONG_INFO"]);
                        return;
                    }

                case PlannerStates.FAVORITES:
                    {
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CHOOSE_FAVORITES_TYPE"], replyMarkup: ReplyKeyboards.Favorites(cache.Lang, lang));
                        break;
                    }

                case PlannerStates.SEND_STAFF_FAVORITES:
                    {
                        menu = await DbManipulations.SendFavorites(chat_id);
                        if (menu is not null && menu.Count > 0)
                        {
                            cache.State = PlannerStates.SELECT_STAFF_FAVORITES;
                            message_for_user = lang[cache.Lang]["CHOOSE_SPECIALIST"];
                            break;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["EMPTY_LIST"]);
                            cache.State = PlannerStates.FAVORITES;
                            return;
                        }

                    }
                case PlannerStates.SELECT_STAFF_FAVORITES:
                    {
                        string staff_name = msg.Substring(msg.LastIndexOf(')') + 2);
                        cache.Staff = msg != lang[cache.Lang]["back"] ? staff_name : cache.Staff;
                        viStaffInfo staffInfo = await DbManipulations.GetStaffInfo(cache.Staff);
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

                case PlannerStates.SEND_ORG_FAVORITES:
                    {
                        menu = await DbManipulations.SendOrgFavorites(chat_id);
                        if (menu is not null && menu.Count > 0)
                        {
                            cache.State = PlannerStates.SELECT_ORG_FAVORITES;
                            message_for_user = lang[cache.Lang]["CHOOSE_ORG"];
                            break;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["EMPTY_LIST"]);
                            cache.State = PlannerStates.FAVORITES;
                            return;
                        }
                    }
                case PlannerStates.SELECT_ORG_FAVORITES:
                    {
                        cache.Organization = msg;
                        var spec = await DbManipulations.GetSpecializationByOrganization(cache.Organization);
                        if (spec is not null)
                        {
                            menu = await DbManipulations.SendCategoriesByOrgName(cache.Organization, cache.Lang);
                            message_for_user = lang[cache.Lang]["CHOOSE_CAT"];
                            cache.Specialization = spec;
                            cache.State = PlannerStates.STAFF;
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
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["settings"], replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
                        break;
                    }
                case PlannerStates.CHANGE_PHONE:
                    {
                        if (Utils.Utils.CheckPhone(msg))
                        {
                            await DbManipulations.UpdateUserPhone(chat_id, msg);
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CONFIRMED"]);
                            cache.State = PlannerStates.SETTINGS;
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["settings"], replyMarkup: ReplyKeyboards.Settings(cache.Lang, lang));
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
                        menu = await DbManipulations.SendSpecializations(cache.Lang);
                        message_for_user = lang[cache.Lang]["CHOOSE_SERVICE"];
                        cache.State = PlannerStates.ORGANIZATION;
                        break;
                    }
                case PlannerStates.ORGANIZATION:
                    {
                        var check_spec = await DbManipulations.CheckSpecialization(msg);
                        if (!check_spec && msg != lang[cache.Lang]["back"]) return;

                        cache.Specialization = msg != lang[cache.Lang]["back"] ? msg : cache.Specialization;
                        message_for_user = lang[cache.Lang]["CHOOSE_ORG"];
                        menu = await DbManipulations.SendOrganizations(cache.Specialization);
                        cache.State = PlannerStates.CATEGORY;
                        break;
                    }
                case PlannerStates.CATEGORY:
                    {
                        var check_org = await DbManipulations.CheckOrganization(msg);
                        if (!check_org && msg != lang[cache.Lang]["back"]) return;

                        cache.Organization = msg != lang[cache.Lang]["back"] ? msg : cache.Organization;
                        menu = await DbManipulations.SendCategoriesByOrgName(cache.Organization, cache.Lang);
                        cache.State = PlannerStates.STAFF;
                        message_for_user = lang[cache.Lang]["CHOOSE_CAT"];
                        break;
                    }
                case PlannerStates.STAFF:
                    {
                        var check_category = await DbManipulations.CheckCategory(msg);
                        if (!check_category && msg != lang[cache.Lang]["back"]) return;

                        cache.Category = msg != lang[cache.Lang]["back"] ? msg : cache.Category;
                        menu = await DbManipulations.GetStaffByCategory(cache.Category, cache.Organization);
                        cache.State = msg != lang[cache.Lang]["back"] ? PlannerStates.CHOOSE_DATE : cache.State;
                        message_for_user = await DbManipulations.GetOrgMessage(cache.Organization, cache.Lang);
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
                        await DbManipulations.RegisterUserSettings(chat_id, cache.Lang);
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache);
                        await Utils.Utils.SendOrder(cache, bot, DbManipulations, chat_id);

                        if (!await DbManipulations.CheckFavorites(cache.Staff, chat_id))
                        {
                            cache.State = PlannerStates.ADD_FAVORITES;
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ADD_FAVORITES"], replyMarkup: ReplyKeyboards.SendConfirmKeyboards(cache.Lang, lang));
                            break;
                        }
                        cache.State = PlannerStates.MAIN_MENU;
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

                        cache.Staff = null;
                        cache.State = PlannerStates.MAIN_MENU;
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                        break;
                    }
                default:
                    break;
            }

            if (menu is not null)
            {
                var buttons = ReplyKeyboards.SendMenuKeyboards(menu);

                if (await DbManipulations.CheckSpecializationType(cache.Organization) && cache.State == PlannerStates.STAFF)
                    buttons.Add(new List<KeyboardButton> { new KeyboardButton(lang[cache.Lang]["analysis"]) });

                if (cache.State > 0) buttons.Add(new List<KeyboardButton> { new KeyboardButton(lang[cache.Lang]["back"]) });

                ReplyKeyboardMarkup markup = new(buttons) { ResizeKeyboard = true };
                await bot.SendTextMessageAsync(chat_id, message_for_user, replyMarkup: markup);
                return;
            }
        }

        private async Task OnCommands(TelegramBotValuesModel cache, string command, long chat_id, IDbManipulations db)
        {
            if (command == lang[cache.Lang]["back"])
            {
                switch (cache.State)
                {
                    case PlannerStates.SETTINGS:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
                            break;
                        }
                    case PlannerStates.CHANGE_LANG:
                    case PlannerStates.CHANGE_NAME:
                    case PlannerStates.CHANGE_PHONE:
                        {
                            cache.State = PlannerStates.SETTINGS;
                            break;
                        }

                    case PlannerStates.FEEDBACK:
                    case PlannerStates.FAVORITES:
                    case PlannerStates.CHECK_PHONE:
                    case PlannerStates.CHECK_PASSWORD:
                    case PlannerStates.ORGANIZATION:
                        {
                            cache.State = PlannerStates.MAIN_MENU;
                            break;
                        }

                    case PlannerStates.SELECT_ORG_FAVORITES:
                    case PlannerStates.SELECT_STAFF_FAVORITES:
                        {
                            cache.State = PlannerStates.FAVORITES;
                            break;
                        }

                    case PlannerStates.CATEGORY:
                        {
                            cache.State = PlannerStates.SPECIALIZATION;
                            break;
                        }
                    case PlannerStates.STAFF:
                        {
                            cache.State = PlannerStates.ORGANIZATION;
                            break;
                        }
                    case PlannerStates.ANALYSIS:
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
                            await bot.SendTextMessageAsync(chat_id, command, replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CUZY_TIME"], replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
                            return;
                        }
                    case PlannerStates.USERNAME:
                        {
                            cache.State = PlannerStates.PHONE;
                            break;
                        }
                    default:
                        break;
                }
            }

            else if (command == lang[cache.Lang]["favorites"])
                cache.State = PlannerStates.FAVORITES;

            else if (command == lang[cache.Lang]["book"])
                cache.State = PlannerStates.SPECIALIZATION;

            else if (command == lang[cache.Lang]["settings"])
                cache.State = PlannerStates.SETTINGS;

            else if (command == lang[cache.Lang]["change_lang"])
            {
                cache.State = PlannerStates.CHANGE_LANG;
                await bot.SendTextMessageAsync(chat_id, choose_lang, replyMarkup: ReplyKeyboards.SendLanguages());
            }

            else if (command == lang[cache.Lang]["change_name"])
            {
                if (await db.GetUserId(chat_id) == 0)
                {
                    await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["SHOULD_REGISTR"]);
                    cache.State = PlannerStates.MAIN_MENU;
                    return;
                }
                await bot.SendTextMessageAsync(chat_id, command, replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                cache.State = PlannerStates.PREPARE_CHANGE_NAME;
                return;
            }

            else if (command == lang[cache.Lang]["change_phone"])
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

            else if (command == lang[cache.Lang]["feedback"])
            {
                cache.State = PlannerStates.ON_PREPARE_FEEDBACK;
                return;
            }

            else if (command == lang[cache.Lang]["about_us"])
            {
                await bot.SendTextMessageAsync(chat_id, "MALUMOT");
            }

            else if (command == lang[cache.Lang]["contacts"])
            {
                await bot.SendTextMessageAsync(chat_id, "MALUMOT");
            }

            else if (command == lang[cache.Lang]["statistic"])
            {
                await bot.SendTextMessageAsync(chat_id, await Utils.Utils.SendStatistic(db, cache, lang), parseMode: ParseMode.Html);
            }

            else if (command == lang[cache.Lang]["staff_favorites"])
            {
                cache.State = PlannerStates.SEND_STAFF_FAVORITES;
                return;
            }

            else if (command == lang[cache.Lang]["org_favorites"])
            {
                cache.State = PlannerStates.SEND_ORG_FAVORITES;
                return;
            }

            else if (command == lang[cache.Lang]["gen_qr"])
            {
                long staffTgId = await db.GetStaffTelegramId(chat_id);
                if (staffTgId != 0)
                {
                    await GenerateQr(chat_id, cache, db);
                    return;
                }
                cache.State = PlannerStates.PREPARE_CHECK_PHONE;
                await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ON_CHANGE_PHONE"], replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang), parseMode: ParseMode.Html);
            }
            else if (command == lang[cache.Lang]["analysis"])
            {
                await bot.SendTextMessageAsync(chat_id, command, replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang));
                await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CUZY_DATE"], replyMarkup: await InlineKeyboards.SendUserAnalysisDates(chat_id, cache.Organization, db));
                cache.State = PlannerStates.ANALYSIS;
            }
        }
    }
}