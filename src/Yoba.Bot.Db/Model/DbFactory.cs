using LinqToDB;

namespace Yoba.Bot.Db
{
    public class DbFactory : IFactory<YobaBotDB>
    {
        readonly string _connectionString;
        readonly string _provider;

        public DbFactory(string connectionString, string provider = ProviderName.SqlServer2008)
        {
            _connectionString = connectionString;
            _provider = provider;
        }

        public YobaBotDB Create()
        {
            return new YobaBotDB(_provider, _connectionString);
        }
    }
}