using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;
using Telegram.Bot.Types.Enums;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.keyboards
{
    public class TimeKeyboards
    {
        public static async Task<InlineKeyboardMarkup> SendTimeKeyboards(IDbManipulations db, TelegramBotValuesModel value)
        {
            var doctorsDate = await db.GetStaffBusyTime(value);
            int[] staff_avail = await db.CheckStaffAvailability(value.Staff);
            viOrgTimes breakTime = await db.GetOrganizationBreak(value.Organization);
            viOrgTimes workTime = await db.GetOrgWorkTime(value.Organization);

            int dayOfWeek = (int)value.Calendar.DayOfWeek;
            List<string> appointmentTime = new();
            List<DateTime> appointmentDate = new();
            foreach (var times in doctorsDate)
            {
                appointmentTime.Add(times.ToShortTimeString());
                appointmentDate.Add(times.Date);
            }

            TimeSpan time = new(workTime.Start.Value.Hour, workTime.Start.Value.Minute, 0);
            TimeSpan offset = new (0, 30, 0);
            int row_limit = (workTime.End.Value.Hour - workTime.Start.Value.Hour) / 2;
            
            if (staff_avail[dayOfWeek] == 2)
                time = new (breakTime.End.Value.Hour, 0, 0);

            var keyboards = new List<List<InlineKeyboardButton>>();

            for (int row = 0; row < row_limit; row++)
            {
                var times_row = new List<InlineKeyboardButton>();
                for (int col = 0; col < 4; col++)
                {
                    var checkBreakTime = breakTime.Start.HasValue && (breakTime.Start.Value.TimeOfDay <= time) && (time < breakTime.End.Value.TimeOfDay);
                    
                    if (checkBreakTime || appointmentTime.Contains(time.ToString(@"hh\:mm")) && appointmentDate.Contains(value.Calendar.Date))
                    {
                        times_row.Add(InlineKeyboardButton.WithCallbackData(" ", "i"));
                        time = time.Add(offset);
                        continue;
                    }
                    times_row.Add(InlineKeyboardButton.WithCallbackData(time.ToString(@"hh\:mm"), $"TIME; {time:hh\\:mm}"));
                    time = time.Add(offset);
                }
                keyboards.Add(times_row);
            }
            InlineKeyboardMarkup markup = new (keyboards);
            return markup;
        }
        public static async Task OnTimeProcess(CallbackQuery call, ITelegramBotClient bot, IDbManipulations db, LangsModel lang)
        {
            long chat_id = call.Message.Chat.Id;
            string[] data = CalendarKeyboards.SeparateCallbackData(call.Data);
            string action = data[0];
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            
            switch (action)
            {
                case "TIME":
                    {
                        cache.Time = data[1];
                        await bot.EditMessageTextAsync(chat_id, call.Message.MessageId, $"{lang[cache.Lang]["CHOOSEN_TIME"]} <b>{data[1]}</b>", parseMode: ParseMode.Html);
                        if (await db.CheckUser(chat_id))
                        {
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["BID"]);
                            await db.RegistrateUserPlanner(chat_id, cache);
                            await Utils.Utils.SendOrder(cache, bot, db, chat_id);
                            if (!await db.CheckFavorites(cache.Staff, chat_id))
                            {
                                await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["ADD_FAVORITES"], replyMarkup: ReplyKeyboards.SendConfirmKeyboards(cache.Lang, lang));
                                cache.State = PlannerStates.ADD_FAVORITES;
                                break;
                            }
                            cache.State = PlannerStates.NONE;
                            await bot.SendTextMessageAsync(chat_id, lang[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang, lang));
                            break;
                        }
                        else
                        {
                            cache.State = PlannerStates.PHONE;
                            await ReplyKeyboards.RequestContactAsync(bot, chat_id, cache.Lang, lang);
                            break;
                        }
                    }
                default:
                    {
                        await bot.AnswerCallbackQueryAsync(call.Id, lang[cache.Lang]["EMPTY_ICON"], true);
                        break;
                    }
            }
        }
    }            
}
