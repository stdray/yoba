using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.Ordering;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Tests
{
    
    public class ProfileRepositoryTests : IDisposable
    {
        readonly IServiceScope _scope;
        readonly IProfileDao _dao;

        readonly YobaProfile _profile = MakeProfile(id => new YobaProfile
        {
            Id = id,
            MainName = $"User {id}",
            Slivi = 5,
            Loisy = 7,
            Zashkvory = 2
        });

        readonly YobaAttribute _attribute = new YobaAttribute
        {
            Id = Guid.NewGuid(),
            Name = "bar"
        };

        public ProfileRepositoryTests()
        {
            _scope = Setup.GetScope();
            _dao = _scope.ServiceProvider.GetService<IProfileDao>();
        }

        [Fact, Order(1)]
        public async Task Attribute_ShouldBeCreated()
        {
            await _dao.CreateAttribute(_attribute);
            var dst = await _dao.FindAttribute(_attribute.Name);
            dst.Id.Should().Be(_profile.Id);
        }

        [Fact, Order(1)]
        public async Task Profile_ShouldBeCreated()
        {
            await _dao.CreateProfile(_profile);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Id.Should().Be(_profile.Id);
            dst.MainName.Should().Be(_profile.MainName);
            dst.Slivi.Should().Be(_profile.Slivi);
            dst.Loisy.Should().Be(_profile.Loisy);
            dst.Zashkvory.Should().Be(_profile.Zashkvory);
        }

        [Fact, Order(2)]
        public async Task Alias_ShouldBeAddedAndFound()
        {
            const string alias = "foo";
            await _dao.AddAlias(_profile.Id, alias);
            var dst = await _dao.FindProfile(alias);
            dst.Names.Single().Should().Be(alias);
            dst.MainName.Should().Be(_profile.MainName);
            dst.Id.Should().Be(_profile.Id);
        }

        [Fact, Order(2)]
        public async Task Loisy_ShouldBeIncremented()
        {
            await _dao.AddLois(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Loisy.Should().Be(_profile.Loisy + 1);
        }

        [Fact, Order(2)]
        public async Task Zashkvory_ShouldBeIncremented()
        {
            await _dao.AddZashkvor(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Zashkvory.Should().Be(_profile.Zashkvory + 1);
        }

        [Fact, Order(2)]
        public async Task Slivi_ShouldBeIncremented()
        {
            await _dao.AddSliv(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Slivi.Should().Be(_profile.Slivi + 1);
        }

        [Fact, Order(3)]
        public async Task AllProfiles_ShouldBeReturned()
        {
            await _dao.CreateProfile(MakeProfile(id => new YobaProfile
            {
                Id = id,
                MainName = id.ToString()
            }));
            var profiles = await _dao.GetProfiles();
            profiles.Count.Should().Be(2);
            profiles.Any(x => x.Id == _profile.Id).Should().Be(true);
        }
        
        [Fact, Order(3)]
        public async Task AllAttributes_ShouldBeReturned()
        {
            await _dao.CreateAttribute(new YobaAttribute
            {
                Id = Guid.NewGuid(),
                Name = "baz"
            });
            var attributes = await _dao.GetAttributes();
            attributes.Count.Should().Be(2);
            attributes.Any(x => x.Id == _attribute.Id).Should().Be(true);
        }
        
        [Fact, Order(4)]
        public async Task ProfileAttribute_ShouldBeCreatedAndListed()
        {
            var profileAttribute = new YobaProfileAttribute
            {
                Attribute = _attribute,
                Value = "peka",
                ProfileId = _profile.Id,
            };
            await _dao.SetProfileAttribute(profileAttribute);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Attributes.Single().Value.Should().Be(profileAttribute.Value);
            var profileAttributes = await _dao.GetProfileAttributes(_attribute.Name);
            profileAttributes.Count.Should().Be(1);
            profileAttributes.Single().Value.Should().Be(profileAttribute.Value);
        }
        

        static YobaProfile MakeProfile(Func<Guid, YobaProfile> ctor) =>
            ctor(Guid.NewGuid());

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}