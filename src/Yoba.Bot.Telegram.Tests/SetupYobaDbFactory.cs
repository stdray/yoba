using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yoba.Bot.Db;
using Yoba.Bot.DbUp;

namespace Yoba.Bot.Tests
{
    public class SetupYobaDbFactory : YobaDbFactory
    {
        public SetupYobaDbFactory(ILoggerFactory loggerFactory, UpgraderOptions options)
            : base(loggerFactory.CreateLogger<YobaDb>(), Options.Create(new Config
            {
                ConnectionString = options.ConnectionString,
            }))
        {
            var upgrader = new Upgrader(loggerFactory, options);
            upgrader.Upgrade();
        }
    }
}