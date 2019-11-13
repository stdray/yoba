using Microsoft.Extensions.Logging;
using Yoba.Bot.Db;
using Yoba.Bot.DbUp;

namespace Yoba.Bot.Tests
{
    public class SetupYobaDbFactory : YobaDbFactory
    {
        public SetupYobaDbFactory(ILoggerFactory loggerFactory, UpgraderOptions options) : base(
            options.ConnectionString)
        {
            var upgrader = new Upgrader(loggerFactory, options);
            upgrader.Upgrade();
        }
    }
}