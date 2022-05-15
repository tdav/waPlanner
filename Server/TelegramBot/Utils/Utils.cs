﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using waPlanner.ModelViews;
using waPlanner.ModelViews.TelegramViews;

namespace waPlanner.TelegramBot.Utils
{
    public static class Utils
    {
        public static bool CheckPhone(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                string pattern = @"^\+\d{12}$";
                Regex regex = new(pattern, RegexOptions.IgnorePatternWhitespace);
                if (regex.IsMatch(msg))
                    return true;
            }
            return false;
        }

        public static async Task SendOrder(TelegramBotValuesModel cache, ITelegramBotClient bot, IDbManipulations db, long chat_id)
        {
            long group_id = await db.GetOrganizationGroupId(cache.Organization);
            var userInfo = await db.GetUserInfo(chat_id);
            string registerDate = DateTime.Now.Date == userInfo.CreateDate.Date ? "Новый Пользовтель" : userInfo.CreateDate.ToString();
            string lang = cache.Lang == "ru" ? "🇷🇺" : "🇺🇿";
            string order = $"🧾<b>Новое поступление</b>\n\n" +
                           $"Имя пользователя: <b>{userInfo.Name}</b>\n" +
                           $"Платформа: <b>{userInfo.Surname}</b>\n" +
                           $"📞Номер пользователя: <b>{userInfo.Phone}</b>\n" +
                           $"Язык пользователя: <b>{lang}</b>\n" +
                           $"🗓Дата планирования: <b>{cache.Calendar.Date.ToShortDateString()} {cache.Time}</b>\n" +
                           $"📙Забронированный субъект: <b>{cache.Staff}</b>\n" +
                           $"Категория субьекта : <b>{cache.Category}</b>\n" +
                           $"🏬Выбранная организация: <b>{cache.Organization}</b>\n" +
                           $"Дата регистрации: <b>{registerDate}</b>";
            await bot.SendTextMessageAsync(group_id, order, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

        }

        public static async Task<string> SendStatistic(IDbManipulations db, TelegramBotValuesModel cache, LangsModel lang)
        {
            var commonStats = await db.GetCommonStatistic();
            var orgsStats = await db.GetOrgsStatistics();

            string totalStats = $"<b>{lang[cache.Lang]["statistic"]}</b>\n\n" +
                                $"👨‍👩‍👧‍👦{lang[cache.Lang]["PLATFORM_USERS"]} <b>{commonStats.TotalUsersCount}</b>\n" +
                                $"📄{lang[cache.Lang]["TOTAL_BOOKS"]} <b>{commonStats.TotalBooks}</b>\n";

            foreach (var stat in orgsStats)
            {
                totalStats += $"{lang[cache.Lang]["BOOKS_COUNT"]} 🏬<b>{stat.Name}: {stat.Count}</b>\n";
            }
            return totalStats;
        }

        public static bool CheckUserCommand(string msg, TelegramBotValuesModel cache, LangsModel lang)
        {
            if (cache.Lang is not null && (msg == lang[cache.Lang]["back"] || cache.State == PlannerStates.MAIN_MENU ||
                cache.State == PlannerStates.SETTINGS || cache.State == PlannerStates.FAVORITES || cache.State == PlannerStates.STAFF))
                return true;
            return false;
        }

        public static async Task<viAnalysisResult> SendAnalysisResult(long chat_id, TelegramBotValuesModel cache, IDbManipulations db, LangsModel lang)
        {
            var results = await db.GetUserAnalysis(chat_id, cache.Organization);

            if (results is null) return null;

            string userAnalys = $"{lang[cache.Lang]["ANALYS_RESULT"]}\n\n";
            var analys = new viAnalysisResult
            {
                AdInfo = userAnalys + results.AdInfo,
                FileUrl = results.FileUrl
            };
            return analys;
        }
    }
}



