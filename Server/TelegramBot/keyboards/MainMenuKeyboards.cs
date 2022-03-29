using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace waPlanner.TelegramBot.keyboards
{
    public class MainMenuKeyboards
    {
        public static ReplyKeyboardMarkup MainMenu()
        {
            var keyboards = new List<List<KeyboardButton>>();
            var buttons = new List<KeyboardButton>
            {
                new KeyboardButton("Main"),
                new KeyboardButton("Test"),
                new KeyboardButton("Test"),
                new KeyboardButton("Test")
            };
            keyboards.Add(buttons);
            ReplyKeyboardMarkup markup = new (keyboards) { ResizeKeyboard = true};
            return markup;
        }
    }
}
