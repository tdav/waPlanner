using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.keyboards
{
    public class ReplyKeyboards
    {
        public static List<List<KeyboardButton>> SendKeyboards(List<IdValue> items, int columns = 2)
        {
            var keyboards = new List<List<KeyboardButton>>();
            var buttons = new List<KeyboardButton>();

            foreach (var item in items)
            {
                buttons.Add(new KeyboardButton(item.Name));

                if (buttons.Count % columns == 0)
                {
                    keyboards.Add(buttons);
                    buttons = new List<KeyboardButton>();
                }
            }

            if (keyboards.Count != 0)
                keyboards.Add(buttons);
            buttons = new List<KeyboardButton>();
            buttons.Add(new KeyboardButton("⬅️Назад"));
            keyboards.Add(buttons);
            return keyboards;
        }
        public static ReplyKeyboardMarkup MainMenu()
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { "Main", "Test" },
                        new KeyboardButton[] { "Test", "Test" },
                })
            {
                ResizeKeyboard = true
            };

            return markup;
        }
        public static ReplyKeyboardMarkup SendContactKeyboard()
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new[]
                    {
                        KeyboardButton.WithRequestContact("Отправить номер телефона📞")
                    },
                    new[]
                    {
                        new KeyboardButton("⬅️Назад")
                    }
                })
            { ResizeKeyboard = true};
            return markup;
        }
    }
}
