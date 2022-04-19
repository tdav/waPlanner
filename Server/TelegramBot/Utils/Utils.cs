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

        public static async Task SendOrder(TelegramBotValuesModel cache, ITelegramBotClient bot, MyDbContext db)
        {
            long group_id = await DbManipulations.GetGroupId(db, cache.Organization);
            string order = $"Новое поступление\n\n" +
                           $"Имя пользователя: {cache.UserName}\n" +
                           $"Номер пользователя: {cache.Phone}\n" +
                           $"Язык пользователя: {cache.Lang}\n" +
                           $"Дата планирования: {cache.Calendar.Date.ToShortDateString()} {cache.Time}\n" +
                           $"Забронированный субъект: {cache.Staff}\n" +
                           $"Категория субьекта : {cache.Category}\n" +
                           $"Выбранная организация: {cache.Organization}";
            await bot.SendTextMessageAsync(group_id, order, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

        }
    }
}
