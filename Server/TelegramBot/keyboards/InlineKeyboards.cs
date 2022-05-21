using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.keyboards
{
    public class InlineKeyboards
    {
        public static List<List<InlineKeyboardButton>> SendMenuKeyboards(List<IdValue> items, int columns = 2)
        {
            var keyboards = new List<List<InlineKeyboardButton>>();
            var buttons = new List<InlineKeyboardButton>();

            foreach (var item in items)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(item.Value, item.Value));

                if (buttons.Count % columns == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }
            }

            if (keyboards.Count != 0 || keyboards.Count % 2 == 0)
                keyboards.Add(buttons);
            return keyboards;
        }

        public static async Task<InlineKeyboardMarkup> SendUserAnalysisDates(long chat_id, string organization, IDbManipulations db)
        {
            var keyboards = new List<List<InlineKeyboardButton>>();
            
            var dates = await db.GetUserAnalysisDates(chat_id, organization);

            foreach (var item in dates)
            {
                var buttons = new List<InlineKeyboardButton>();
                buttons.Add(InlineKeyboardButton.WithCallbackData(item.ToShortDateString(), item.ToShortDateString()));
                keyboards.Add(buttons);
            }
            return new InlineKeyboardMarkup(keyboards);
        }
    }
}
