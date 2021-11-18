using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MihaZupan;
using NLog.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.Db;
using Yoba.Bot.Entities;
using Yoba.Bot.Telegram;
using Yoba.Bot.Telegram.Middlewares;

namespace Yoba.Bot.App;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseEnvironment(Environment.GetEnvironmentVariable("YobaEnv") ?? Environments.Development)
            .ConfigureAppConfiguration((ctx, c) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment())
                    c.AddUserSecrets<BotService.Config>();
                else
                {
                    var config = Environment.GetEnvironmentVariable("YobaConfPath");
                    if (config != null)
                        c.AddJsonFile(config);
                }
            })
            .ConfigureServices(ConfigureServices);

    static void ConfigureServices(HostBuilderContext host, IServiceCollection sc)
    {
        const string sectionName = "YobaBot";
        var serviceConfig = host.Configuration.GetSection(sectionName).Get<BotService.Config>();

        sc.Configure<BotService.Config>(host.Configuration.GetSection(sectionName));
        sc.Configure<YobaDbFactory.Config>(host.Configuration.GetSection(sectionName));

        sc.AddLogging(lb =>
        {
            lb.ClearProviders();
            lb.SetMinimumLevel(LogLevel.Trace);
            lb.AddNLog();
        });

        sc.AddSingleton<IMiddleware<Message>, LogMiddleware>();
        sc.AddSingleton<IProvider<Message>, TelegramTextProvider>();
        sc.AddSingleton<ITelegramBotClient>(_ => CreateTelegramBotClient(serviceConfig));
        sc.AddSingleton<IRandomGenerator, ThreadLocalRandom>();

        //sc.AddScoped(_ => CreateUpgraderOptions());
        sc.AddSingleton<IYobaDbFactory, YobaDbFactory>();
        sc.AddSingleton<IProfileDao, ProfileDao>();
        sc.AddSingleton<INoteDao, NoteDao>();

        sc.AddSingleton<IController<Message>, SimpleController>();
        sc.AddSingleton<IController<Message>, ProfileController>();
        sc.AddSingleton<IController<Message>, NoteController>();
        sc.AddSingleton<BotHandler<Message>>();

        sc.AddHostedService<BotService>();
    }

    static TelegramBotClient CreateTelegramBotClient(BotService.Config config)
    {
        if (config.Proxy == null)
            return new TelegramBotClient(config.TelegramToken);
        var px = config.Proxy;
        var proxy = string.IsNullOrEmpty(px.Login)
            ? new HttpToSocks5Proxy(px.Host, px.Port)
            : new HttpToSocks5Proxy(px.Host, px.Port, px.Login, px.Password);
        var handler = new HttpClientHandler { Proxy = proxy };
        var httpClient = new HttpClient(handler, true);
        return new TelegramBotClient(config.TelegramToken, httpClient);
    }
}