using LinqToDB;
using LinqToDB.Tools;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Db;

public class ProfileDao : IProfileDao
{
    readonly IYobaDbFactory _factory;

    public ProfileDao(IYobaDbFactory factory)
    {
        _factory = factory;
    }

    public Task CreateProfile(YobaProfile p, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.Profiles.InsertAsync(() => new Profile
        {
            Id = p.Id,
            Loisy = p.Loisy,
            Slivi = p.Slivi,
            Zashkvory = p.Zashkvory,
            CanVote = p.CanVote,
            MainName = p.MainName,
            IsTransport = false,
        }, cancel);
    }

    public Task AddAlias(Guid profileId, string name, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.ProfileNames.InsertAsync(() => new ProfileName
        {
            Name = name,
            ProfileId = profileId,
        }, cancel);
    }

    public Task AddLois(Guid userId, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.Profiles
            .Where(x => x.Id == userId)
            .Set(x => x.Loisy, x => x.Loisy + 1)
            .UpdateAsync(cancel);
    }

    public Task AddZashkvor(Guid userId, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.Profiles
            .Where(x => x.Id == userId)
            .Set(x => x.Zashkvory, x => x.Zashkvory + 1)
            .UpdateAsync(cancel);
    }


    public Task AddSliv(Guid userId, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.Profiles
            .Where(x => x.Id == userId)
            .Set(x => x.Slivi, x => x.Slivi + 1)
            .UpdateAsync(cancel);
    }

    public async Task<IReadOnlyCollection<YobaProfile>> GetProfiles(
        CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return await YobaProfiles(db).ToListAsync(cancel);
    }

    public async Task<YobaProfile> FindProfile(string name, CancellationToken cancel = default)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        using var db = _factory.Create();
        var variants = new []{ name.TrimStart('@'), name};
        var profile = await db.Profiles
            .Where(p => p.MainName.In(variants) 
                        || p.ProfileNames.Any(n => n.Name.In(variants)))
            .Select(x => new YobaProfile
            {
                Id = x.Id,
                Loisy = x.Loisy,
                Slivi = x.Slivi,
                Zashkvory = x.Zashkvory,
                CanVote = x.CanVote,
                MainName = x.MainName
            })
            .SingleOrDefaultAsync(cancel);
        if (profile == null)
            return null;
        var names = await db.ProfileNames
            .Where(x => x.ProfileId == profile.Id)
            .Select(x => x.Name)
            .ToListAsync(cancel);
        var attributes = await YobaProfileAttributes(db)
            .Where(x => x.ProfileId == profile.Id)
            .ToListAsync(cancel);
        profile.Names = names;
        profile.Attributes = attributes;
        return profile;
    }

    public async Task<IReadOnlyCollection<YobaProfileAttribute>> GetProfileAttributes(string name,
        CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return await YobaProfileAttributes(db)
            .Where(x => x.AttributeName == name)
            .ToListAsync(cancel);
    }

    public Task CreateAttribute(YobaAttribute attribute, CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return db.Attributes
            .InsertAsync(() => new Attribute
            {
                Id = attribute.Id,
                Attribute_Column = attribute.Name,
            }, cancel);
    }


    public async Task SetProfileAttribute(YobaProfileAttribute attribute,
        CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        var count = await db.ProfileAttributes
            .Where(x => x.ProfileId == attribute.ProfileId && x.AttributeId == attribute.AttributeId)
            .Set(x => x.Value, _ => attribute.Value)
            .UpdateAsync(cancel);
        if (count == 1)
            return;
        if (count > 1)
            throw new InvalidOperationException("Conflict");
        await db.ProfileAttributes.InsertAsync(() => new ProfileAttribute
        {
            Value = attribute.Value,
            AttributeId = attribute.AttributeId,
            ProfileId = attribute.ProfileId,
        }, cancel);
    }

    public async Task<YobaAttribute> FindAttribute(string name,
        CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return await YobaAttributes(db)
            .Where(x => x.Name == name)
            .SingleOrDefaultAsync(cancel);
    }

    static IQueryable<YobaProfile> YobaProfiles(YobaDb db) =>
        db.Profiles.Select(x => new YobaProfile
        {
            Id = x.Id,
            Loisy = x.Loisy,
            Slivi = x.Slivi,
            Zashkvory = x.Zashkvory,
            CanVote = x.CanVote,
            MainName = x.MainName
        });

    public async Task<IReadOnlyCollection<YobaAttribute>> GetAttributes(
        CancellationToken cancel = default)
    {
        using var db = _factory.Create();
        return await YobaAttributes(db).ToListAsync(cancel);
    }

    static IQueryable<YobaAttribute> YobaAttributes(YobaDb db) =>
        db.Attributes
            .Select(x => new YobaAttribute
            {
                Id = x.Id,
                Name = x.Attribute_Column
            });

    static IQueryable<YobaProfileAttribute> YobaProfileAttributes(YobaDb db) =>
        db.ProfileAttributes
            .Select(x => new YobaProfileAttribute
            {
                Value = x.Value,
                ProfileId = x.ProfileId,
                AttributeId = x.AttributeId,
                AttributeName = x.Attribute.Attribute_Column,
                ProfileName = x.Profile.MainName
            });
}