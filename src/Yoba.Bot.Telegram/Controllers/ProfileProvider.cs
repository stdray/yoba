//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Telegram.Bot.Types;
//using Yoba.Bot.Db;
//
//namespace Yoba.Bot.Telegram.Controllers
//{
//    public class ProfileProvider : IProvider<Message, Profile>
//    {
//        readonly IFactory<YobaBotDB> _dbFactory;
//        readonly Func<Message, string> _nameSelector;
//
//        public ProfileProvider(IFactory<YobaBotDB> dbFactory, Func<Message, string> nameSelector)
//        {
//            _dbFactory = dbFactory;
//            _nameSelector = nameSelector;
//        }
//
//        public async Task<Profile> Provide(string key, Message message, Profile defaultValue = default(Profile),
//            CancellationToken cancellation = default(CancellationToken))
//        {
//            var name = _nameSelector(message);
//            using (var rep = new ProfileRepository(_dbFactory.Create()))
//            {
//                var id = await rep.GetIdByName(name, cancellation);
//                if (!id.HasValue)
//                    return defaultValue;
//                var profile = await rep.GetProfile(id.Value, cancellation);
//                return profile;
//            }
//        }
//    }
//}