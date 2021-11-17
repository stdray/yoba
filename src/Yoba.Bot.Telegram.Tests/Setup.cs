using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Xunit;
using Yoba.Bot.Db;
using Yoba.Bot.DbUp;
using Yoba.Bot.Entities;
using Yoba.Bot.Telegram;

[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]

namespace Yoba.Bot.Tests
{
    public static class Setup
    {
        static readonly Lazy<IServiceProvider> ServiceProvider =
            new Lazy<IServiceProvider>(Configure, LazyThreadSafetyMode.ExecutionAndPublication);

        public static T GetService<T>() => ServiceProvider.Value.GetService<T>();

        public static IServiceScope GetScope() => ServiceProvider.Value.CreateScope();

        public static Request<Message> Message(string txt, string username = null) =>
            new Request<Message>(new Message
            {
                Text = txt,
                Chat = new Chat { Id = 1488 },
                From = new User
                {
                    Username = username
                }
            });

        static IServiceProvider Configure()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton(CreateLoggerFactory());
            sc.AddSingleton<IProvider<Message>, TelegramTextProvider>();
            sc.AddSingleton(CreateTelegramBotClient());
            sc.AddSingleton(CreateRandomGenerator(1));
            sc.AddSingleton<SimpleController>();

            sc.AddScoped(_ => CreateUpgraderOptions());
            sc.AddScoped<IYobaDbFactory, SetupYobaDbFactory>();
            sc.AddScoped<IProfileDao, ProfileDao>();
            sc.AddScoped<INoteDao, NoteDao>();
            sc.AddScoped<ProfileController>();
            sc.AddScoped<NoteController>();
            return sc.BuildServiceProvider();
        }

        static ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(f => { f.AddConsole(); });

        static UpgraderOptions CreateUpgraderOptions()
        {
            var conStr = $"Data Source={Guid.NewGuid()}, Mode=memory;";
            return new UpgraderOptions { ConnectionString = conStr, AutoCreateDb = true, };
        }

        static IRandomGenerator CreateRandomGenerator(int returnNum)
        {
            var random = A.Fake<IRandomGenerator>();
            A.CallTo(() => random.Next(A<int>.Ignored, A<int>.Ignored))
                .Returns(returnNum);
            return random;
        }

        static ITelegramBotClient CreateTelegramBotClient()
        {
            var telegram = A.Fake<ITelegramBotClient>();
            A.CallTo(telegram)
                .Where(call => call.Method.Name == nameof(telegram.MakeRequestAsync))
                .WithReturnType<Task<Message>>()
                .ReturnsLazily(call =>
                {
                    var request = call.Arguments.Get<SendMessageRequest>(0);
                    var chatId = request?.ChatId.Identifier 
                                 ?? throw new NullReferenceException(nameof(request.ChatId));
                    var message = new Message
                    {
                        Text = request.Text,
                        Chat = new Chat { Id = chatId }
                    };
                    return Task.FromResult(message);
                });
            return telegram;
        }
    }
}