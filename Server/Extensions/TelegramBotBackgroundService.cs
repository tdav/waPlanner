﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using waPlanner.TelegramBot;
using waPlanner.TelegramBot.Services;

namespace waPlanner.Extensions
{
    public class TelegramBotBackgroundService : BackgroundService
    {
        private int messageOffset = 0;
        private readonly TelegramBotClient bot;
        private readonly IServiceProvider provider;
        private readonly IBotService service;
        private readonly ILogger<TelegramBotBackgroundService> logger;

        public TelegramBotBackgroundService(ILogger<TelegramBotBackgroundService> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;

            this.bot = new TelegramBotClient(Config.TOKEN);
            this.service = provider.GetRequiredService<IBotService>();
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, token);

                    var updatesRequest = new GetUpdatesRequest
                    {
                        Offset = messageOffset,
                        Timeout = 0,
                        AllowedUpdates = { },
                        Limit = 1000
                    };

                    var list = await bot.MakeRequestAsync(updatesRequest, token);

                    if (list != null && list.Length > 0)
                    {
                        await service.HandleAsync(bot, list, token);

                        messageOffset = list[^1].Id + 1;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }
    }
}
