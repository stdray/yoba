using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Yoba.Bot.Db;

namespace Yoba.Bot.Telegram.Controllers
{
    public class ProfileRepository : IDisposable
    {
        readonly YobaBotDB _db;

        public ProfileRepository(YobaBotDB db)
        {
            _db = db;
        }

        public async Task<Guid?> GetIdByName(string name, CancellationToken cancellation = default(CancellationToken))
        {
            var profileName = await _db.Profiles
                .Select(x => new ProfileName {ProfileId = x.Id, Name = x.MainName})
                .Union(_db.ProfileNames)
                .SingleOrDefaultAsync(x => x.Name == name, cancellation);
            return profileName?.ProfileId;
        }

        public async Task<Profile> GetProfile(Guid profileId,
            CancellationToken cancellation = default(CancellationToken))
        {
            var profile = await _db.Profiles
                .LoadWith(x => x.ProfileNames)
                .LoadWith(x => x.ProfileAttributes)
                .LoadWith(x => x.ProfileAttributes.First().Attribute)
                .SingleAsync(x => x.Id == profileId, cancellation);
            return profile;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}