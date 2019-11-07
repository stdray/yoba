namespace Yoba.Bot.Db
{
    public partial class YobaBotDB
    {
        public YobaBotDB(string provider, string connectionString) : base(provider, connectionString)
        {
            InitDataContext();
            InitMappingSchema();
        }
    }
}