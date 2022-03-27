using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot
{
    public class Keyboards
    {
        public static List<List<KeyboardButton>> SendKeyboards(List<IdValue> items, int columns = 2)
        {
            var keyboards =  new List<List<KeyboardButton>>();
            var buttons = new List<KeyboardButton>();

            foreach (var item in items)
            {
                buttons.Add(new KeyboardButton(item.Name));

                if (buttons.Count % columns == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<KeyboardButton>();
                }
            }

            if (keyboards.Count != 0)
                keyboards.Add(buttons);

            return keyboards;
        }
        public static List<List<InlineKeyboardButton>> SendCalendar(DateTime date, int month)
        {
            string[] daysWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            string IGNORE = "i";
            var keyboards = new List<List<InlineKeyboardButton>>();
            var buttons = new List<InlineKeyboardButton>();

            //first row
            buttons.Add((InlineKeyboardButton.WithCallbackData("⏪", "PREV-YEAR")));
            buttons.Add((InlineKeyboardButton.WithCallbackData(date.Year.ToString(), IGNORE)));
            buttons.Add((InlineKeyboardButton.WithCallbackData("⏩", "NEXT-YEAR")));
            keyboards.Add(buttons);

            //second row for days
            buttons = new List<InlineKeyboardButton>();
            foreach (string weekDay in daysWeek)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(weekDay.ToString(), IGNORE));
            }
            keyboards.Add(buttons);

            //day of the months
            buttons = new List<InlineKeyboardButton>();
            var padLeftDays = (int)date.DayOfWeek;
            var currentDay = date;
            

            var iterations = DateTime.DaysInMonth(date.Year, month) + padLeftDays;
            int j = 1;

            for (int i = 1; i < iterations;)
            {
                if (j < padLeftDays)
                {
                    j++;
                    buttons.Add(InlineKeyboardButton.WithCallbackData("x", IGNORE));
                    
                }
                else
                {
                    i++;
                    buttons.Add(InlineKeyboardButton.WithCallbackData(currentDay.Day.ToString(), "day"));

                    if (buttons.Count % 7 == 0)
                    {
                        keyboards.Add(buttons);
                        buttons = new List<InlineKeyboardButton>();
                    }
                    currentDay = currentDay.AddDays(1);
                }
                if (buttons.Count != 0 && currentDay.Day <= DateTime.DaysInMonth(date.Year, date.Month))
                    buttons.Add(InlineKeyboardButton.WithCallbackData("x", IGNORE));
            }
            

            keyboards.Add(buttons);

            return keyboards;
        }
    }
}
