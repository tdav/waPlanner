using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using waPlanner.ModelViews;


namespace waPlanner.TelegramBot.keyboards
{
    public class CalendarKeyboards
    {
        public static InlineKeyboardMarkup SendCalendar(DateTime date)
        {
            string[] daysWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            string IGNORE = $"i;{DateTime.Now}";

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

            int iterations = DateTime.DaysInMonth(date.Year, date.Month) + padLeftDays;

            for (int i = 1; i <= iterations; i++)
            {
                if (i < padLeftDays)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(" ", IGNORE));
                    continue;
                }
                if (currentDate.Month == date.Month)
                    if(currentDate.Day < DateTime.Today.Day)
                    {
                        buttons.Add(InlineKeyboardButton.WithCallbackData($"<s>{currentDate.Day.ToString()}</s>", $"DAY; {currentDate}"));
                        continue;
                    }
                    buttons.Add(InlineKeyboardButton.WithCallbackData(currentDate.Day.ToString(), $"DAY; {currentDate}"));

                if (buttons.Count % 7 == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }

                currentDate = currentDate.AddDays(1);

                if ((int)currentDate.DayOfWeek < 7 && currentDate.Month != date.Month && keyboards.Count <= 6)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(" ", IGNORE));
                    iterations++;
                }
            }
            keyboards.Add(buttons);

            // thirth row for month
            buttons = new List<InlineKeyboardButton>();
            buttons.Add(InlineKeyboardButton.WithCallbackData("◀", $"PREV-MONTH; {date.AddMonths(-1)}"));
            buttons.Add(InlineKeyboardButton.WithCallbackData(Microsoft.VisualBasic.DateAndTime.MonthName(date.Month), IGNORE));
            buttons.Add(InlineKeyboardButton.WithCallbackData("▶️", $"NEXT-MONTH; {date.AddMonths(1)}"));
            keyboards.Add(buttons);
            InlineKeyboardMarkup calendar = new(keyboards);
            return calendar;
        }
        public static string[] SeparateCallbackData(string data)
        {
            return data.Split(";");
        }
        public static async Task OnCalendarProcess(CallbackQuery call, ReplyKeyboardMarkup back, MyDbContext db)
        {
            long chat_id = call.Message.Chat.Id;
            int messageId = call.Message.MessageId;

            string[] data = SeparateCallbackData(call.Data);
            string action = data[0];
            
            var bot = handlers.Handlers.Bot_;
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            

            if (cache.State == PlannerStates.CHOOSE_DATE)
            {
                DateTime date = DateTime.Parse(data[1]);
                switch (action)
                {
                    case "NEXT-MONTH":
                        {
                            await bot.EditMessageReplyMarkupAsync(chat_id, messageId, SendCalendar(date));
                            break;
                        }
                    case "PREV-MONTH":
                        {
                            if(date.Month >= DateTime.Now.Month)
                            {
                                await bot.EditMessageReplyMarkupAsync(chat_id, messageId, SendCalendar(date));
                                break;
                            }
                            break;
                            
                        }
                    case "DAY":
                        {
                            if (date >= DateTime.Today)
                            {
                                cache.State = PlannerStates.CHOOSE_TIME;
                                cache.Calendar = date;
                                try
                                {
                                    await bot.DeleteMessageAsync(chat_id, messageId);
                                }
                                catch
                                {
                                
                                }
                                await bot.SendTextMessageAsync(chat_id, $"Выбрана дата: <b>{date.ToShortDateString()}</b>", replyMarkup: back, parseMode: ParseMode.Html);
                                await bot.SendTextMessageAsync(chat_id, "Выберите удобное для вас время.", replyMarkup: TimeKeyboards.SendTimeKeyboards(db, cache));
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
