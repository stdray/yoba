using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot.Entities
{
    public interface IProfileDao
    {
        Task CreateProfile(YobaProfile profile, CancellationToken cancel = default);
        Task AddAlias(Guid profileId, string name, CancellationToken cancel = default);
        Task AddLois(Guid userId, CancellationToken cancel = default);
        Task AddZashkvor(Guid userId, CancellationToken cancel = default);
        Task AddSliv(Guid userId, CancellationToken cancel = default);
        Task<IReadOnlyCollection<YobaProfile>> GetProfiles(CancellationToken cancel = default);
        Task<YobaProfile> FindProfile(string name, CancellationToken cancel = default);

        Task<IReadOnlyCollection<YobaProfileAttribute>> GetProfileAttributes(string name,
            CancellationToken cancel = default);

        Task SetProfileAttribute(YobaProfileAttribute attribute, CancellationToken cancel = default);
        Task CreateAttribute(YobaAttribute attribute, CancellationToken cancel = default);
        Task<YobaAttribute> FindAttribute(string name, CancellationToken cancel = default);
        Task<IReadOnlyCollection<YobaAttribute>> GetAttributes(CancellationToken cancel = default);
    }
}