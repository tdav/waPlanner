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

            List<string> appointmentTime = new();
            List<DateTime> appointmentDate = new();
            foreach (var times in doctorsDate)
            {
                appointmentTime.Add(times.ToShortTimeString());
                appointmentDate.Add(times.Date);
            }

            TimeSpan time = new(10, 0, 0);
            TimeSpan offset = new(0, 30, 0);

            var keyboards = new List<List<InlineKeyboardButton>>();

            for (int row = 0; row < 4; row++)
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
            if (cache.State == PlannerStates.CHOOSE_TIME)
            {
                switch (action)
                {
                    case "TIME":
                        {
                            cache.Time = data[1];
                            await bot.EditMessageTextAsync(chat_id, call.Message.MessageId, $"Выбрано время:<b>{data[1]}</b>", parseMode:ParseMode.Html);
                            if (await DbManipulations.CheckUser(chat_id, db))
                            {
                                List<IdValue> services = await DbManipulations.GetAllGlobalCats(db);
                                var servicesButtons = ReplyKeyboards.SendKeyboards(services);
                                ReplyKeyboardMarkup reply = new(servicesButtons) { ResizeKeyboard = true };
                                await bot.SendTextMessageAsync(chat_id, "Ваша заявка принята, ждите звонка от оператора", replyMarkup: reply);
                                await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                                cache.State = PlannerStates.NONE;
                                break;
                            }
                            else
                            {
                                cache.State = PlannerStates.PHONE;
                                await ReplyKeyboards.RequestContactAsync(bot, chat_id);
                                break;
                            }
                            
                        }
                    default:
                        {
                            await bot.AnswerCallbackQueryAsync(call.Id, cacheTime: 600);
                            break;
                        }
                }
            }
        }            
    }
}
