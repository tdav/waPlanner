using System;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using waPlanner.ModelViews;
using waPlanner.TelegramBot.handlers;

namespace waPlanner.TelegramBot
{
    public class StartBot
    {
        private static readonly TelegramBotClient telegramBotClient = new(Config.TOKEN);
        public static TelegramBotClient? Bot = telegramBotClient;

        public static void BotStart()
        {
            var s = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "lang.json");
            Program.langs = Newtonsoft.Json.JsonConvert.DeserializeObject<LangsModel>(s);

            using var cts = new CancellationTokenSource();
            ReceiverOptions options = new() { AllowedUpdates = { } };
            Bot.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, options, cts.Token);
        }
    }
}
