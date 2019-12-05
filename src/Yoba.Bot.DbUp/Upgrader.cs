using DbUp;
using Microsoft.Extensions.Logging;

namespace Yoba.Bot.DbUp
{
//    public class Program
//    {
//        static int Main(string[] args)
//        {
//            var config = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json")
//                .Build();
//
//            var options = new UpgraderOptions
//            {
//                ConnectionString = config.GetConnectionString("YobaDb"),
//                AutoCreateDb = true
//            };
//            EnsureDatabase.For.SqlDatabase(connectionString);
//
//            var upgrader = new Upgrader(options);
//
//            var result = upgrader.PerformUpgrade();
//
//            if (!result.Successful)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine(result.Error);
//                Console.ResetColor();
//#if DEBUG
//                Console.ReadLine();
//#endif
//                return -1;
//            }
//
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine("Success!");
//            Console.ResetColor();
//            return 0;
//        }
//    }

    public class Upgrader
    {
        readonly UpgraderOptions _options;

        public Upgrader(ILoggerFactory loggerFactory, UpgraderOptions options)
        {
            _options = options;
            Log = loggerFactory.CreateLogger(GetType());
        }

        ILogger Log { get; }

        public bool Upgrade()
        {
            Log.LogInformation("Begin upgrade");
            var upgrader = DeployChanges.To
                .SQLiteDatabase(_options.ConnectionString)
                .WithScriptsEmbeddedInAssembly(GetType().Assembly)
                .LogTo(new UpgradeLog(Log))
                .Build();
            var result = upgrader.PerformUpgrade();
            if (result.Successful)
                Log.LogInformation("End upgrade");
            else
                Log.LogError(result.Error, "Fail upgrade");
            return result.Successful;
        }
    }
}