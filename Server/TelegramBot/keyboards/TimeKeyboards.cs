using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;
using Telegram.Bot.Types.Enums;
using waPlanner.Database;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.TelegramBot.keyboards
{
    public class TimeKeyboards
    {
        public static async Task<InlineKeyboardMarkup> SendTimeKeyboards(MyDbContext db, TelegramBotValuesModel value)
        {
            var doctorsDate = await DbManipulations.GetStaffBusyTime(db, value);
            int[] staff_avail = await DbManipulations.CheckStaffAvailability(db, value.Staff);
            int dayOfWeek = (int)value.Calendar.DayOfWeek;

            List<string> appointmentTime = new();
            List<DateTime> appointmentDate = new();
            foreach (var times in doctorsDate)
            {
                appointmentTime.Add(times.ToShortTimeString());
                appointmentDate.Add(times.Date);
            }

            TimeSpan time = new(10, 0, 0);
            TimeSpan offset = new(0, 30, 0);
            int row_limit = 4;

            if(staff_avail[dayOfWeek] == 2)
            {
                time = new(14, 0, 0);
                row_limit = 2;
            }

            var keyboards = new List<List<InlineKeyboardButton>>();

            for (int row = 0; row < row_limit; row++)
            {
                var times_row = new List<InlineKeyboardButton>();
                for (int col = 0; col < 4; col++)
                {
                    if (appointmentTime.Contains(time.ToString(@"hh\:mm")) && appointmentDate.Contains(value.Calendar.Date))
                    {
                        times_row.Add(InlineKeyboardButton.WithCallbackData(" ", $"i"));
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
        public static async Task OnTimeProcess(CallbackQuery call, ITelegramBotClient bot, MyDbContext db)
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
                        await bot.EditMessageTextAsync(chat_id, call.Message.MessageId, $"{Program.langs[cache.Lang]["CHOOSEN_TIME"]} <b>{data[1]}</b>", parseMode: ParseMode.Html);
                        if (await DbManipulations.CheckUser(chat_id, db))
                        {
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["BID"]);
                            await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                            await Utils.Utils.OnFinish(cache, bot, db);
                            if (!await DbManipulations.CheckFavorites(db, cache.Staff, chat_id))
                            {
                                await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["ADD_FAVORITES"], replyMarkup: ReplyKeyboards.SendConfirmKeyboards(cache.Lang));
                                cache.State = PlannerStates.ADD_FAVORITES;
                                break;
                            }
                            cache.State = PlannerStates.NONE;
                            await bot.SendTextMessageAsync(chat_id, Program.langs[cache.Lang]["NONE"], replyMarkup: ReplyKeyboards.MainMenu(cache.Lang));
                            break;
                        }
                        else
                        {
                            cache.State = PlannerStates.PHONE;
                            await ReplyKeyboards.RequestContactAsync(bot, chat_id, cache.Lang);
                            break;
                        }
                    }
                default:
                    {
                        await bot.AnswerCallbackQueryAsync(call.Id, Program.langs[cache.Lang]["EMPTY_ICON"], true);
                        break;
                    }
            }
        }
    }            
}
