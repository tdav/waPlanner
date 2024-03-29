﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.Utils;
using System.Globalization;

namespace waPlanner.TelegramBot.keyboards
{
    public class CalendarKeyboards
    {
        public static InlineKeyboardMarkup Calendar(ref DateTime date, string lg)
        {
            string[] daysWeek = { "Du", "Se", "Ch", "Pa", "Ju", "Sh", "Ya" };
             
            if (lg == "ru")
                daysWeek = new string[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

            string IGNORE = $"i;i";

            var keyboards = new List<List<InlineKeyboardButton>>();
            var buttons = new List<InlineKeyboardButton>();

            ////first row for days
            foreach (string weekDay in daysWeek)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(weekDay.ToString(), IGNORE));
            }
            keyboards.Add(buttons);

            //second row days of the months
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
            buttons.Add(InlineKeyboardButton.WithCallbackData(new DateTime(1,date.Month, 1).ToString("MMMM", CultureInfo.GetCultureInfo("ru-RU")), IGNORE));
            buttons.Add(InlineKeyboardButton.WithCallbackData("▶️", $"NEXT-MONTH; {date.AddMonths(1)}"));
            keyboards.Add(buttons);
            InlineKeyboardMarkup calendar = new(keyboards);
            return calendar;
        }

        public static string[] SeparateCallbackData(string data)
        {
            return data.Split(";");
        }

        public static async Task OnCalendarProcess(CallbackQuery call, IDbManipulations db, ITelegramBotClient bot, LangsModel lang, TelegramBotValuesModel cache)
        {
            long chat_id = call.Message.Chat.Id;
            int messageId = call.Message.MessageId;

            string[] data = SeparateCallbackData(call.Data);
            string action = data[0];
            
            DateTime.TryParse(data[1], out var date);
            switch (action)
            {
                case "NEXT-MONTH":
                    {
                        await bot.EditMessageReplyMarkupAsync(chat_id, messageId, Calendar(ref date, cache.Lang));
                        break;
                    }
                case "PREV-MONTH":
                    {
                        if(date.Month >= DateTime.Now.Month)
                        {
                            await bot.EditMessageReplyMarkupAsync(chat_id, messageId, Calendar(ref date, cache.Lang));
                            break;
                        }
                        break;
                            
                    }
                case "DAY":
                    {
                        if (date < DateTime.Today)
                        {
                            await bot.AnswerCallbackQueryAsync(call.Id, lang[cache.Lang]["OLD_DATE"], true);
                            return;
                        }

                        int[] staff_avail = await db.CheckStaffAvailability(cache);
                        if (staff_avail[(int)date.DayOfWeek] == 0)
                        {
                            await bot.AnswerCallbackQueryAsync(call.Id, lang[cache.Lang]["BUSY_DATE"], true);
                            return;
                        }

                        cache.State = PlannerStates.CHOOSE_TIME;
                        cache.Calendar = date;
                        try
                        {
                            await bot.DeleteMessageAsync(chat_id, messageId);
                        }
                        catch { }

                        await bot.SendTextMessageAsync(chat_id, $"{lang[cache.Lang]["CHOOSEN_DATE"]} <b>{date.ToShortDateString()}</b>",
                            replyMarkup: ReplyKeyboards.BackButton(cache.Lang, lang), parseMode: ParseMode.Html);
                        await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["CUZY_TIME"], replyMarkup: await TimeKeyboards.SendTimeKeyboards(db, cache));
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

        public static async Task SendCalendar(ITelegramBotClient bot, long chat_id, string lg, LangsModel lang)
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            await bot.SendTextMessageAsync(chat_id, lang[lg]["CUZY_DATE"], replyMarkup: ReplyKeyboards.BackButton(lg, lang));
            await bot.SendTextMessageAsync(chat_id, lang[lg]["CALENDAR"], replyMarkup: Calendar(ref date, lg));
        }
    }
}
