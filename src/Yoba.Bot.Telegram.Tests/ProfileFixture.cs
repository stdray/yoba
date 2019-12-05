using System;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Tests
{
    public class ProfileFixture : ServiceScopeFixture
    {
        public YobaProfile Profile { get; } = MakeProfile(id => new YobaProfile
        {
            Id = id,
            MainName = $"User_{id}",
            Slivi = 5,
            Loisy = 7,
            Zashkvory = 2,
            CanVote = true
        });
        
        public YobaAttribute Attribute { get; } = new YobaAttribute
        {
            Id = Guid.NewGuid(),
            Name = "bar"
        };


        public static YobaProfile MakeProfile(Func<Guid, YobaProfile> ctor) =>
            ctor(Guid.NewGuid());
    }
}