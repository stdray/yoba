using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.App
{
    public class BotService : IHostedService
    {
        readonly IServiceProvider _serviceProvider;
        readonly IOptions<BotServiceConfig> _config;
        readonly ITelegramBotClient _telegram;
        CancellationToken _cancel = CancellationToken.None;

        public BotService(IServiceProvider serviceProvider, IOptions<BotServiceConfig> config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _telegram = _serviceProvider.GetService<ITelegramBotClient>();
        }

        public Task StartAsync(CancellationToken cancel)
        {
            _telegram.OnMessage += OnMessage;
            _telegram.StartReceiving(cancellationToken: cancel);
            _cancel = cancel;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancel)
        {
            _cancel = cancel;
            _telegram.StopReceiving();
            _telegram.OnMessage -= OnMessage;
            return Task.CompletedTask;
        }

        void OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Chat.Id != _config.Value.GroupChatId
                || msg.ForwardFrom != null 
                || msg.ForwardFromChat != null 
                || msg.From.IsBot)
            {
                return;
            }
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<BotHandler<Message>>();
                handler.Handle(new Request<Message>(e.Message), _cancel);
            }
        }
    }
}