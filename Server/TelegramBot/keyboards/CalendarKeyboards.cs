using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;


namespace waPlanner.TelegramBot.keyboards
{
    public class CalendarKeyboards
    {
        public static InlineKeyboardMarkup SendCalendar(DateTime date, int month)
        {
            string[] daysWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            string IGNORE = "i";

            var keyboards = new List<List<InlineKeyboardButton>>();
            var buttons = new List<InlineKeyboardButton>();

            ////first row for days
            foreach (string weekDay in daysWeek)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(weekDay.ToString(), IGNORE));
            }
            keyboards.Add(buttons);

            //second row day of the months
            buttons = new List<InlineKeyboardButton>();
            int padLeftDays = (int)date.DayOfWeek != 0 ? (int)date.DayOfWeek : 7;
            var currentDate = date;

            int iterations = DateTime.DaysInMonth(date.Year, month) + padLeftDays;

            for (int i = 1; i <= iterations; i++)
            {
                if (i < padLeftDays)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(" ", IGNORE));
                    continue;
                }
                if (currentDate.Month == month)
                    buttons.Add(InlineKeyboardButton.WithCallbackData(currentDate.Day.ToString(), $"DAY; {currentDate.Month}; {currentDate.Year}; {currentDate.Day}"));

                if (buttons.Count % 7 == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }

                currentDate = currentDate.AddDays(1);

                if ((int)currentDate.DayOfWeek < 7 && currentDate.Month != month && keyboards.Count <= 6)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(" ", IGNORE));
                    iterations++;
                }
            }
            keyboards.Add(buttons);

            // thirth row for month
            buttons = new List<InlineKeyboardButton>();
            buttons.Add(InlineKeyboardButton.WithCallbackData("◀", $"PREV-MONTH; {date.Month}"));
            buttons.Add(InlineKeyboardButton.WithCallbackData(Microsoft.VisualBasic.DateAndTime.MonthName(month), IGNORE));
            buttons.Add(InlineKeyboardButton.WithCallbackData("▶️", $"NEXT-MONTH; {date.Month}"));
            keyboards.Add(buttons);
            InlineKeyboardMarkup calendar = new(keyboards);
            return calendar;
        }
        public static string[] SeparateCallbackData(string data)
        {
            return data.Split(";");
        }
        public static async Task OnCalendarProcess(CallbackQuery call, ReplyKeyboardMarkup back)
        {
            long chat_id = call.Message.Chat.Id;
            string[] data = SeparateCallbackData(call.Data);
            string action = data[0];
            var bot = handlers.Handlers.Bot_;
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            if (cache.State == PlannerStates.CHOOSE_DATE)
            {
                switch (action)
                {
                    case "NEXT-MONTH":
                        {
                            DateTime date = new DateTime(DateTime.Now.Year, int.Parse(data[1]) + 1, 1);
                            await bot.EditMessageReplyMarkupAsync(chat_id, call.Message.MessageId, SendCalendar(date, int.Parse(data[1]) + 1));
                            break;
                        }
                    case "PREV-MONTH":
                        {
                            DateTime date = new DateTime(DateTime.Now.Year, int.Parse(data[1]) - 1, 1);
                            await bot.EditMessageReplyMarkupAsync(chat_id, call.Message.MessageId, SendCalendar(date, int.Parse(data[1]) - 1));
                            break;
                        }
                    case "DAY":
                        {
                            int day = int.Parse(data[3]);
                            DateTime selectedDate = new(int.Parse(data[2]), int.Parse(data[1]), day);
                            Console.WriteLine(selectedDate);
                            Console.WriteLine(DateTime.Now);
                            if (DateTime.Now <= selectedDate)
                            {
                                cache.State = PlannerStates.CHOOSE_TIME;
                                cache.Calendar = selectedDate;
                                try
                                {
                                    await bot.DeleteMessageAsync(chat_id, call.Message.MessageId);
                                }
                                catch
                                {
                                
                                }
                                await bot.SendTextMessageAsync(chat_id, $"Выбрана дата: {selectedDate.ToShortDateString()}", replyMarkup: back);
                                await bot.SendTextMessageAsync(chat_id, "Выберите удобное для вас время.", replyMarkup: TimeKeyboards.SendTimeKeyboards());
                                return;
                            }
                            await bot.AnswerCallbackQueryAsync(call.Id, "Нельзя выбирать старую дату!", true);
                            break;
                        }
                    case "i":
                        {
                            await bot.AnswerCallbackQueryAsync(call.Id);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
