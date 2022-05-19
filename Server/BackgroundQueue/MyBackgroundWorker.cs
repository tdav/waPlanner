using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using waPlanner.ModelViews;

namespace waPlanner.BackgroundQueue
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly ISendDocumentsQueue<SendDocumentsModel> queue;

        private readonly IServiceScopeFactory scopeFactory;

        public MyBackgroundWorker(IServiceScopeFactory _scopeFactory, ISendDocumentsQueue<SendDocumentsModel> queue)
        {
            this.queue = queue;
            scopeFactory = _scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            var bot = scopeFactory.CreateScope().ServiceProvider.GetService<ITelegramBotClient>();

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var item in queue.GetAll())
                {

                    var ba = await File.ReadAllBytesAsync(item.FilePath);
                    using (var ms = new MemoryStream(ba))
                    {
                        var file = new InputOnlineFile(ms, item.User + ".pdf"); ;
                        await bot.SendDocumentAsync(item.ChatId, file, caption: item.Caption, parseMode: ParseMode.Html);
                    }
                }

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
