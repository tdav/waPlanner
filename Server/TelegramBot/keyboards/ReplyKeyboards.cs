using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using waPlanner.ModelViews;


namespace waPlanner.TelegramBot.keyboards
{
    public class ReplyKeyboards
    {
        public static ReplyKeyboardMarkup SendLanguages()
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { "Русский🇷🇺" },
                        new KeyboardButton[] { "O'zbekcha🇺🇿" }
                })
            {
                ResizeKeyboard = true
            };

            return markup;
        }

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
          
            if (keyboards.Count != 0 || keyboards.Count % 2 == 0)
                keyboards.Add(buttons);
            return keyboards;
        }

        public static ReplyKeyboardMarkup MainMenu(string lg)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { Program.langs[lg]["book"] },
                        new KeyboardButton[] { Program.langs[lg]["favorites"], Program.langs[lg]["settings"] },
                        new KeyboardButton[] { Program.langs[lg]["feedback"], Program.langs[lg]["about_us"] },
                        new KeyboardButton[] { Program.langs[lg]["contacts"] },
                })
            {
                ResizeKeyboard = true
            };

            return markup;
        }

        public static ReplyKeyboardMarkup Settings(string lg)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { Program.langs[lg]["change_name"], Program.langs[lg]["change_lang"] },
                        new KeyboardButton[] { Program.langs[lg]["change_phone"], Program.langs[lg]["back"] }
                })
            {
                ResizeKeyboard = true
            };
            return markup;
        }

        public static async Task<Message> RequestContactAsync(ITelegramBotClient bot, long chat_id, string lg)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new[]
                    {
                        KeyboardButton.WithRequestContact(Program.langs[lg]["SEND_CONTACT"])
                    },
                    new[]
                    {
                        new KeyboardButton(Program.langs[lg]["back"])
                    }
                })
            { ResizeKeyboard = true};
            return await bot.SendTextMessageAsync(chat_id, Program.langs[lg]["CONTACT_MESSAGE"],
                        replyMarkup: markup, parseMode: ParseMode.Html); ;
        }

        public static ReplyKeyboardMarkup SendConfirmKeyboards(string lg)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new KeyboardButton[]{ Program.langs[lg]["YES"], Program.langs[lg]["NO"] }
                })
            { ResizeKeyboard = true };
            return markup;
        }
    }
}
