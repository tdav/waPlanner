using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;


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
                buttons.Add(InlineKeyboardButton.WithCallbackData(item.Name, item.Name));

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
    }
}
