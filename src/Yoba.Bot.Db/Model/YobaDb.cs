namespace Yoba.Bot.Db
{
    public partial class YobaDb
    {
        public YobaDb(string provider, string connectionString) : base(provider, connectionString)
        {
            InitDataContext();
            InitMappingSchema();
        }
    }
}