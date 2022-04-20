using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.keyboards
{
    public class CalendarKeyboards
    {
        public static InlineKeyboardMarkup Calendar(ref DateTime date)
        {
            string[] daysWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            string IGNORE = $"i;i";

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
                    buttons.Add(InlineKeyboardButton.WithCallbackData(currentDate.Day.ToString(), $"DAY; {currentDate}"));

                if (buttons.Count % 7 == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }

                currentDate = currentDate.AddDays(1);

                if ((int)currentDate.DayOfWeek < 7 && currentDate.Month != date.Month && keyboards.Count < 7)
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
            
            var bot = handlers.Handlers.bot;
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            
            DateTime.TryParse(data[1], out var date);
            switch (action)
            {
                case "NEXT-MONTH":
                    {
                        await bot.EditMessageReplyMarkupAsync(chat_id, messageId, Calendar(ref date));
                        break;
                    }
                case "PREV-MONTH":
                    {
                        if(date.Month >= DateTime.Now.Month)
                        {
                            await bot.EditMessageReplyMarkupAsync(chat_id, messageId, Calendar(ref date));
                            break;
                        }
                        break;
                            
                    }
                case "DAY":
                    {
                        if (date >= DateTime.Today)
                        {
                            int[] staff_avail = await DbManipulations.CheckStaffAvailability(db, cache.Staff);
                            if (staff_avail[(int)date.DayOfWeek] == 0)
                            {
                                await bot.AnswerCallbackQueryAsync(call.Id, Program.langs[cache.Lang]["BUSY_DATE"], true);
                                return;
                            }
                            var freeDay = await DbManipulations.CheckFreeDay(db, cache.Staff, date);
                            if (freeDay < 16)
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
                                await bot.SendTextMessageAsync(chat_id, $"{Program.langs[cache.Lang]["CHOOSEN_DATE"]} <b>{date.ToShortDateString()}</b>", replyMarkup: back, parseMode: ParseMode.Html);
                                await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["CUZY_TIME"], replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
                                return;
                            }
                            else
                            {
                                await bot.AnswerCallbackQueryAsync(call.Id, Program.langs[cache.Lang]["BOOKED_ALL_DAY"], true);
                                return;
                            }
                        
                        }
                        await bot.AnswerCallbackQueryAsync(call.Id, Program.langs[cache.Lang]["OLD_DATE"], true);
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

        public static async Task SendCalendar(ITelegramBotClient bot, long chat_id, ReplyKeyboardMarkup back, string lg)
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            await bot.SendTextMessageAsync(chat_id, Program.langs[lg]["CUZY_DATE"], replyMarkup: back);
            await bot.SendTextMessageAsync(chat_id, Program.langs[lg]["CALENDAR"], replyMarkup: Calendar(ref date));
        }
    }
}
