using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.App
{
    public class BotService : IHostedService
    {
        readonly IServiceProvider _serviceProvider;
        readonly IOptions<Config> _config;
        readonly ITelegramBotClient _telegram;
        readonly ILogger<BotService> _logger;
        CancellationTokenSource _cancellation;
        // CancellationToken _cancel = CancellationToken.None;

        public BotService(IServiceProvider serviceProvider, IOptions<Config> config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _telegram = _serviceProvider.GetService<ITelegramBotClient>();
            _logger = _serviceProvider.GetService<ILogger<BotService>>();
        }

        public Task StartAsync(CancellationToken cancel)
        {
            _cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            _telegram.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: _cancellation.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken _)
        {
            _cancellation.Cancel();
            return Task.CompletedTask;
        }
        
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancel)
        {
            if (update.Message != null)
            {
                var msg = update.Message;
                if (_config.Value.GroupChatId?.Contains(msg.Chat.Id) != true
                    || msg.ForwardFrom != null
                    || msg.ForwardFromChat != null
                    || msg.From?.IsBot == true)
                {
                    return;
                }
                using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_cancellation.Token, cancel);
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetService<BotHandler<Message>>();
                await handler.Handle(new Request<Message>(msg), linkedToken.Token);
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancel)
        {
            _logger.LogError(exception, "Some error");
            return Task.CompletedTask;
        }

        public class Config
        {
            public string TelegramToken { get; set; }
            public long[] GroupChatId { get; set; }
            public Socks5ProxyConfig Proxy { get; set; }
            public string ConnectionString { get; set; }
        }
    }
}