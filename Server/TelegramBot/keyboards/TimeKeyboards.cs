using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.keyboards
{
    public class TimeKeyboards
    {
        public static InlineKeyboardMarkup SendTimeKeyboards()
        {
            TimeSpan time = new TimeSpan(10, 0, 0);
            TimeSpan offset = new TimeSpan(0, 30, 0);

            var keyboards = new List<List<InlineKeyboardButton>>();

            for (int row = 0; row < 4; row++)
            {
                var times_row = new List<InlineKeyboardButton>();
                for (int col = 0; col < 4; col++)
                {
                    times_row.Add(InlineKeyboardButton.WithCallbackData(time.ToString(@"hh\:mm"), $"TIME; {time.ToString(@"hh\:mm")}"));
                    time = time.Add(offset);
                }
                keyboards.Add(times_row);
            }
            InlineKeyboardMarkup markup = new (keyboards);
            return markup;
        }
        public static async Task OnTimeProcess(CallbackQuery call)
        {
            long chat_id = call.Message.Chat.Id;
            string[] data = CalendarKeyboards.SeparateCallbackData(call.Data);
            string action = data[0];

            var bot = handlers.Handlers.Bot_;

            if (action == "TIME")
            {
                var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
                cache.State = PlannerStates.PHONE;
                cache.Time = data[1];
                //Send message for user
            }
        }
    }
}
