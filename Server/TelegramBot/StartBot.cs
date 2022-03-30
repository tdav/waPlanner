using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using waPlanner.TelegramBot.handlers;

namespace waPlanner.TelegramBot
{
    public class StartBot
    {
        public static void BotStart()
        {
            TelegramBotClient? Bot = new TelegramBotClient(Config.TOKEN);
            using var cts = new CancellationTokenSource();
            ReceiverOptions options = new() { AllowedUpdates = { } };
            Bot.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, options, cts.Token);
        }
    }
}
