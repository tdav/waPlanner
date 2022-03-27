using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using waPlanner.TelegramBot.handlers;

namespace waPlanner
{
    public class Program
    {
        public static Dictionary<long, object> Cache = new Dictionary<long, object>();
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            try
            {
                ConfigureLogging();
                CreateHostBuilder(args)
                    .Build()
                    .Run();
                
            }
            catch (Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            TelegramBotClient? Bot = new TelegramBotClient(Config.TOKEN);
            using var cts = new CancellationTokenSource();
            ReceiverOptions options = new() { AllowedUpdates = { } };
            Bot.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, options, cts.Token);

            return Host.CreateDefaultBuilder(args)
                      .ConfigureWebHostDefaults(webBuilder =>
                      {
                          webBuilder
                           .UseStartup<Startup>()
                           .UseKestrel();
                      })
                      .ConfigureAppConfiguration(configuration =>
                      {
                          configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                      })
                      .UseSerilog();
        }

        private static void ConfigureLogging()
        {
            
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
