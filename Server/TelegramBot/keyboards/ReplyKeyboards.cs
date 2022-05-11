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

        public static List<List<KeyboardButton>> SendMenuKeyboards(List<IdValue> items, int columns = 2)
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

        public static ReplyKeyboardMarkup MainMenu(string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { lang[lg]["book"] },
                        new KeyboardButton[] { lang[lg]["favorites"], lang[lg]["settings"] },
                        new KeyboardButton[] { lang[lg]["feedback"], lang[lg]["about_us"] },
                        new KeyboardButton[] { lang[lg]["contacts"], lang[lg]["statistic"] },
                        new KeyboardButton[] { lang[lg]["gen_qr"] },
                })
            {
                ResizeKeyboard = true
            };

            return markup;
        }

        public static ReplyKeyboardMarkup Settings(string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                        new KeyboardButton[] { lang[lg]["change_name"], lang[lg]["change_lang"] },
                        new KeyboardButton[] { lang[lg]["change_phone"], lang[lg]["back"] }
                })
            {
                ResizeKeyboard = true
            };
            return markup;
        }

        public static async Task<Message> RequestContactAsync(ITelegramBotClient bot, long chat_id, string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new[]
                    {
                        KeyboardButton.WithRequestContact(lang[lg]["SEND_CONTACT"])
                    },
                    new[]
                    {
                        new KeyboardButton(lang[lg]["back"])
                    }
                })
            { ResizeKeyboard = true};
            return await bot.SendTextMessageAsync(chat_id, lang[lg]["CONTACT_MESSAGE"],
                        replyMarkup: markup, parseMode: ParseMode.Html); ;
        }

        public static ReplyKeyboardMarkup SendConfirmKeyboards(string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new KeyboardButton[]{ lang[lg]["YES"], lang[lg]["NO"] }
                })
            { ResizeKeyboard = true };
            return markup;
        }

        public static ReplyKeyboardMarkup BackButton(string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new KeyboardButton[]{ lang[lg]["back"] }
                })
            { ResizeKeyboard = true };
            return markup;
        }
        
        public static ReplyKeyboardMarkup Favorites(string lg, LangsModel lang)
        {
            ReplyKeyboardMarkup markup = new(
                new[]
                {
                    new KeyboardButton[]{ lang[lg]["org_favorites"], lang[lg]["staff_favorites"] },
                    new KeyboardButton[]{ lang[lg]["back"] }
                })
            { ResizeKeyboard = true };
            return markup;
        }
    }
}
