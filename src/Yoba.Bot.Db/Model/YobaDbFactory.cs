using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Data;

namespace Yoba.Bot.Db
{
    public class YobaDbFactory : IYobaDbFactory
    {
        readonly string _provider;

        public YobaDbFactory(string connectionString, string provider = ProviderName.SQLiteClassic)
        {
            ConnectionString = connectionString;
            _provider = provider;
            
            DataConnection.TurnTraceSwitchOn();
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (x, y) => Console.WriteLine("{0}{1}", x, y);
        }

        public string ConnectionString { get; }

        public YobaDb Create()
        {
            return new YobaDb(_provider, ConnectionString);
        }
    }
}