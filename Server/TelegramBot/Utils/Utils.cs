using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using waPlanner.ModelViews;

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
            if (cache.Lang is not null)
            {
                if (msg == lang[cache.Lang]["back"])
                    return true;

                switch (cache.State)
                {
                    case PlannerStates.SETTINGS:
                    case PlannerStates.FAVORITES:
                    case PlannerStates.STAFF:
                    case PlannerStates.MAIN_MENU:
                        {
                            return true;
                        }
                }
                return false;
            }
            return false;
        }

        //public static Document MergePDFs(IEnumerable<string> fileNames, string targetPdf)
        //{
        //    using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
        //    {
        //        Document document = new Document();
        //        PdfCopy pdf = new PdfCopy(document, stream);
        //        PdfReader reader = null;
        //        try
        //        {
        //            document.Open();
        //            foreach (string file in fileNames)
        //            {
        //                reader = new PdfReader($"{AppDomain.CurrentDomain.BaseDirectory}wwwroot{ Path.DirectorySeparatorChar}"
        //                + file);
        //                pdf.AddDocument(reader);
        //                reader.Close();
        //            }
        //            return document;
        //        }
        //        catch (Exception)
        //        {
        //            if (reader != null)
        //            {
        //                reader.Close();
        //            }
        //            return null;
        //        }
        //        finally
        //        {
        //            if (document != null)
        //            {
        //                document.Close();
        //            }
        //        }
        //    }
        //}

        public static async Task SendAnalysisResult(long chat_id, TelegramBotValuesModel cache, IDbManipulations db, LangsModel lang, DateTime date, ITelegramBotClient bot)
        {
            var results = await db.GetUserAnalysis(chat_id, cache.Organization, date);

            //MergePDFs(results, "C:/Users/Elina/dotnet/waPlanner/Server/bin/Debug/net6.0/wwwroot/store/analysis/new1.pdf");

            if (results is null)
            {
                await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["EMTY_RESULT"]);
                return;
            }

            foreach (var result in results)
            {
                Startup.queue.Enqueue(new SendDocumentsModel() 
                { 
                    FilePath = $"{AppDomain.CurrentDomain.BaseDirectory}wwwroot/{result.FileUrl}", 
                    ChatId = chat_id, 
                    User = result.User,
                    Caption = result.AdInfo 
                });
            }
            
        }    
    }
}



