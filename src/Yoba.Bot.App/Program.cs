using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.Db;
using Yoba.Bot.Entities;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.App
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var host = builder.Build();
            await host.RunAsync();
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.AddDebug();
                });

        static void ConfigureServices(HostBuilderContext host, IServiceCollection sc)
        {
            const string sectionName = "YobaBot";
            var config = new BotServiceConfig();
            host.Configuration.GetSection(sectionName).Bind(config);

            sc.Configure<BotServiceConfig>(host.Configuration.GetSection(sectionName));
            sc.AddSingleton<IProvider<Message>, TelegramTextProvider>();
            sc.AddSingleton<ITelegramBotClient>(_ => CreateTelegramBotClient(config));
            sc.AddSingleton<IRandomGenerator, ThreadLocalRandom>();

            //sc.AddScoped(_ => CreateUpgraderOptions());
            sc.AddScoped<IYobaDbFactory>(_ => new YobaDbFactory(config.ConnectionString));
            sc.AddScoped<IProfileDao, ProfileDao>();
            sc.AddScoped<INoteDao, NoteDao>();

            sc.AddSingleton<IController<Message>, SimpleController>();
            sc.AddScoped<IController<Message>,ProfileController>();
            sc.AddScoped<IController<Message>,NoteController>();
            sc.AddScoped<BotHandler<Message>>();
            sc.AddHostedService<BotService>();
        }

        static TelegramBotClient CreateTelegramBotClient(BotServiceConfig config)
        {
            if (config.Proxy == null)
                return new TelegramBotClient(config.TelegramToken);
            var px = config.Proxy;
            var proxy = string.IsNullOrEmpty(px.Login)
                ? new HttpToSocks5Proxy(px.Host, px.Port)
                : new HttpToSocks5Proxy(px.Host, px.Port, px.Login, px.Password);
            return new TelegramBotClient(config.TelegramToken, proxy);
        }
    }
}