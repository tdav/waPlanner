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


namespace waPlanner.TelegramBot.handlers.staff
{
    public class OnStaffsStateChanged
    {
        public static async Task OnStateChange(long chat_id, MyDbContext db, ITelegramBotClient Bot_, Message message,
            List<IdValue> menu, ReplyKeyboardMarkup back)
        {
            string msg = message.Text;

        }
    }
}
