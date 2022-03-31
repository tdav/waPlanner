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
        public static async Task OnTimeProcess(CallbackQuery call, ITelegramBotClient bot, MyDbContext db)
        {
            long chat_id = call.Message.Chat.Id;
            string[] data = CalendarKeyboards.SeparateCallbackData(call.Data);
            string action = data[0];
            var cache = Program.Cache[chat_id] as TelegramBotValuesModel;
            if (cache.State == PlannerStates.CHOOSE_TIME)
            {
                if (action == "TIME")
                {
                    cache.Time = data[1];
                    await bot.EditMessageTextAsync(chat_id, call.Message.MessageId, $"Выбрано время: {data[1]}");
                    if (DbManipulations.CheckUser(chat_id, db))
                    {
                        await bot.SendTextMessageAsync(chat_id, "Ваша заявка принята, ждите звонка от оператора", replyMarkup: ReplyKeyboards.MainMenu());
                        await DbManipulations.RegistrateUserPlanner(chat_id, cache, db);
                        cache.State = PlannerStates.NONE;
                        return;
                    }
                    cache.State = PlannerStates.PHONE;
                    await bot.SendTextMessageAsync(chat_id, "Отправьте ваш действительный номер телефона, " +
                        "нажав на кнопку <b>(Отправить номер телефона📞)</b> или введите в следующем типе: <b>+998 xx xxx xxx xxx</b>",
                        replyMarkup: ReplyKeyboards.SendContactKeyboard(), parseMode: ParseMode.Html);
                }
            }
        }            
    }
}
