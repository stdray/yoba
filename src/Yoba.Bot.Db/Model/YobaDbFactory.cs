using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Yoba.Bot.Db;

public class YobaDbFactory : IYobaDbFactory
{
    static int _logSet = 0;
    readonly IOptions<Config> _config;

    public YobaDbFactory(ILogger<YobaDb> log, IOptions<Config> config)
    {
        _config = config;
        if (_config.Value.LogSqlStatements
            && Interlocked.CompareExchange(ref _logSet, 1, 0) == 0)
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (message, displayName, level) =>
                log.LogDebug("[{level}] {displayName}: {message}", level, displayName, message);
        }
    }


    public YobaDb Create()
    {
        return new YobaDb(_config.Value.Provider, _config.Value.ConnectionString);
    }

    public class Config
    {
        public string ConnectionString { get; set; }
        public bool LogSqlStatements { get; set; }
        public string Provider { get; set; } = ProviderName.SQLiteClassic;
    }
}