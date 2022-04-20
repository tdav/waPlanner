using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using waPlanner.Database;
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
            string registerDate = DateTime.Now.Date == userInfo.CreateDate ? "Новый Пользовтель" : userInfo.CreateDate.ToString();
            string lang = cache.Lang == "🇷🇺" ? cache.Lang : "🇺🇿";
            string order = $"<b>Новое поступление</b>🧾\n\n" +
                           $"Имя пользователя: <b>{userInfo.Name}</b>\n" +
                           $"Платформа: <b>{userInfo.Surname}</b>\n" +
                           $"Номер пользователя📞: <b>{userInfo.Phone}</b>\n" +
                           $"Язык пользователя: <b>{lang}</b>\n" +
                           $"Дата планирования🗓: <b>{cache.Calendar.Date.ToShortDateString()} {cache.Time}</b>\n" +
                           $"Забронированный субъект📙: <b>{cache.Staff}</b>\n" +
                           $"Категория субьекта : <b>{cache.Category}</b>\n" +
                           $"Выбранная организация🏬: <b>{cache.Organization}</b>\n" +
                           $"Дата регистрации: <b>{registerDate}</b>";
            await bot.SendTextMessageAsync(group_id, order, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

        }
    }
}
