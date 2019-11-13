using LinqToDB;

namespace Yoba.Bot.Db
{
    public class YobaDbFactory : IYobaDbFactory
    {
        readonly string _provider;

        public YobaDbFactory(string connectionString, string provider = ProviderName.SQLite)
        {
            ConnectionString = connectionString;
            _provider = provider;
        }

        public string ConnectionString { get; }

        public YobaDb Create()
        {
            return new YobaDb(_provider, ConnectionString);
        }
    }
}