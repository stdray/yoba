using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Yoba.Bot.Telegram.Controllers;

namespace Yoba.Bot.Tests
{
    public class Setup
    {
        public static readonly Lazy<IServiceProvider> ServiceProvider =
            new Lazy<IServiceProvider>(Configure, LazyThreadSafetyMode.ExecutionAndPublication);

        public static T GetService<T>() => ServiceProvider.Value.GetService<T>();

        public static Request<Message> Message(string txt) =>
            new Request<Message>(new Message
            {
                Text = txt,
                Chat = new Chat {Id = 1488}
            });

        static IServiceProvider Configure()
        {
            var sc = new ServiceCollection();
            sc.AddScoped(_ => CreateTelegramBotClient());
            sc.AddScoped<SimpleCommandController>();
            return sc.BuildServiceProvider();
        }

        static ITelegramBotClient CreateTelegramBotClient()
        {
            var messages = new List<Message>();
            var telegram = A.Fake<ITelegramBotClient>();

            A.CallTo(() => telegram.SendTextMessageAsync(
                    A<ChatId>.Ignored,
                    A<string>.Ignored,
                    A<ParseMode>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored,
                    A<int>.Ignored,
                    A<IReplyMarkup>.Ignored,
                    A<CancellationToken>.Ignored))
                .ReturnsLazily((
                    ChatId chatId,
                    string text,
                    ParseMode parseMode,
                    bool disableWebPagePreview,
                    bool disableNotification,
                    int replyToMessageId,
                    IReplyMarkup replyMarkup,
                    CancellationToken cancellationToken) => Task.FromResult(new Message
                {
                    Text = text,
                    Chat = new Chat {Id = chatId.Identifier}
                }));
            return telegram;
        }
    }
}