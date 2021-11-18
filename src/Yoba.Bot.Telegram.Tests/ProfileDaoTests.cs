using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Tests;

public class ProfileDaoTests : IClassFixture<ProfileFixture>
{
    readonly IProfileDao _dao;
    readonly YobaAttribute _attribute;
    readonly YobaProfile _profile;

    public ProfileDaoTests(ProfileFixture fixture)
    {
        _dao = fixture.Scope.ServiceProvider.GetService<IProfileDao>();
        _attribute = fixture.Attribute;
        _profile = fixture.Profile;
    }

    [Fact]
    public async Task Test()
    {
        //Attribute_ShouldBeCreated
        {
            await _dao.CreateAttribute(_attribute);
            var dst = await _dao.FindAttribute(_attribute.Name);
            dst.Id.Should().Be(_attribute.Id);
        }
        //Profile_ShouldBeCreated
        {
            await _dao.CreateProfile(_profile);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Id.Should().Be(_profile.Id);
            dst.MainName.Should().Be(_profile.MainName);
            dst.Slivi.Should().Be(_profile.Slivi);
            dst.Loisy.Should().Be(_profile.Loisy);
            dst.Zashkvory.Should().Be(_profile.Zashkvory);
        }
        //Alias_ShouldBeAddedAndFound
        {
            const string alias = "foo";
            await _dao.AddAlias(_profile.Id, alias);
            var dst = await _dao.FindProfile(alias);
            dst.Names.Single().Should().Be(alias);
            dst.MainName.Should().Be(_profile.MainName);
            dst.Id.Should().Be(_profile.Id);
        }
        //Loisy_ShouldBeIncremented
        {
            await _dao.AddLois(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Loisy.Should().Be(_profile.Loisy + 1);
        }
        //Zashkvory_ShouldBeIncremented
        {
            await _dao.AddZashkvor(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Zashkvory.Should().Be(_profile.Zashkvory + 1);
        }
        //Slivi_ShouldBeIncremented
        {
            await _dao.AddSliv(_profile.Id);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Slivi.Should().Be(_profile.Slivi + 1);
        }
        //AllProfiles_ShouldBeReturned
        {
            await _dao.CreateProfile(new YobaProfile
            {
                Id = Guid.NewGuid(), MainName = "kekek"
            });
            var profiles = await _dao.GetProfiles();
            profiles.Count.Should().Be(2);
            profiles.Any(x => x.Id == _profile.Id).Should().Be(true);
        }
        //AllAttributes_ShouldBeReturned
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
        //ProfileAttribute_ShouldBeCreatedAndListed
        {
            var profileAttribute = new YobaProfileAttribute
            {
                AttributeId = _attribute.Id,
                AttributeName = _attribute.Name,
                Value = "peka",
                ProfileId = _profile.Id,
                ProfileName = _profile.MainName,
            };

            await _dao.SetProfileAttribute(profileAttribute);
            var dst = await _dao.FindProfile(_profile.MainName);
            dst.Attributes.Single().Value.Should().Be(profileAttribute.Value);
            var profileAttributes = await _dao.GetProfileAttributes(_attribute.Name);
            profileAttributes.Count.Should().Be(1);
            profileAttributes.Single().Value.Should().Be(profileAttribute.Value);
        }
    }
}