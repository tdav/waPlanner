using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.Utils
{
    public class OnStateChanged
    {
        public static async Task OnMenuAction(List<IdValue> menu, long chat_id, ITelegramBotClient bot, string msg, TelegramBotValuesModel value)
        {
            ReplyKeyboardMarkup markup = new(Keyboards.SendKeyboards(menu)) { ResizeKeyboard = true };

            switch (value.State)
            {
                case PlannerStates.CATEGORY:
                    {
                        value.State = PlannerStates.DOCTORS;
                        value.Category = msg;
                        Program.Cache[chat_id] = value;
                        await bot.SendTextMessageAsync(chat_id, "Выберите Врача", replyMarkup: markup);
                        break;
                    }
                case PlannerStates.DOCTORS:
                    {
                        break;
                    }
                default:
                    value.State = PlannerStates.CATEGORY;
                    value.Category = msg;
                    Program.Cache[chat_id] = value;
                    await bot.SendTextMessageAsync(chat_id, "Выберите категорию", replyMarkup: markup);
                    break;
            }
        }     
    }
}
