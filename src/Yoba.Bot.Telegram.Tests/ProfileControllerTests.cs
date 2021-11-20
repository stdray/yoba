using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Xunit;
using Yoba.Bot.Entities;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.Tests;

public class ProfileControllerTests : IClassFixture<ProfileFixture>
{
    readonly ProfileController _controller;
    readonly IProfileDao _dao;
    readonly YobaAttribute _attribute;
    readonly YobaProfile _profile;

    public ProfileControllerTests(ProfileFixture fixture)
    {
        _controller = fixture.Scope.ServiceProvider.GetService<ProfileController>();
        _dao = fixture.Scope.ServiceProvider.GetService<IProfileDao>();
        _attribute = fixture.Attribute;
        _profile = fixture.Profile;
    }

    [Fact]
    public async Task Test()
    {
        //NewAttribute
        {
            await Handle($"ёба создай атрибут {_attribute.Name}");
            var attribute = await _dao.FindAttribute(_attribute.Name);
            attribute.Should().NotBeNull();
        }

        //NewProfile
        {
            await Handle($"ёба создай профиль {_profile.MainName}");
            var profile = await _dao.FindProfile(_profile.MainName);
            profile.Should().NotBeNull();
        }

        //AddProfileAlias
        {
            const string alias = "foo";
            await Handle($"ёба алиас {alias} для {_profile.MainName}");
            var dst = await _dao.FindProfile(alias);
            dst.Names.Single().Should().Be(alias);
            dst.MainName.Should().Be(_profile.MainName);
        }

        //AddProfileKarma
        {
            var from = "user1";
            await _dao.CreateProfile(new YobaProfile
            {
                Id = Guid.NewGuid(),
                MainName = "user1",
                CanVote = true
            });
            var before = await _dao.FindProfile(_profile.MainName);
            // Lois from unknown user don't change karma
            var result = await Handle($"ёба лойс {_profile.MainName}");
            result.Response.Text.Should().Be("Вы не можете голосовать");
            // Lois from self to self don't change karma
            result = await Handle($"ёба лойс {from}", from);
            result.Response.Text.Should().Be("Нельзя менять карму самому себе");
            var after = await _dao.FindProfile(_profile.MainName);
            after.Loisy.Should().Be(before.Loisy);
            // Lois, sliv, Zashkvory should be incremented
            result = await Handle($"ёба ЖлойС {_profile.MainName}", from);
            result.Response.Text.Should().Be("Лойс из жалости поставлен");
            await Handle($"ёба слив {_profile.MainName}", from);
            await Handle($"ёба зашквор {_profile.MainName}", from);
            after = await _dao.FindProfile(_profile.MainName);
            after.Loisy.Should().Be(before.Loisy + 1);
            after.Slivi.Should().Be(before.Slivi + 1);
            after.Zashkvory.Should().Be(before.Zashkvory + 1);
        }

        //SetProfileAttributeValue
        var attributeValue = Guid.NewGuid().ToString();
        {
            await Handle($"ёба атрибут {_attribute.Name} для {_profile.MainName} : {attributeValue}");
            var after = await _dao.FindProfile(_profile.MainName);
            after.Attributes.Single(x => x.AttributeName == _attribute.Name).Value.Should().Be(attributeValue);
        }

        //ListAttributes
        {
            var result = await Handle("ёба покажи список атрибутов");
            result.Response.Text.Should().Contain($"{_attribute.Name}");
        }

        //ShowAttributeValues
        {
            var result = await Handle($"ёба покажи атрибут {_attribute.Name}");
            result.Response.Text.Should().Contain($"{_profile.MainName}");
            result.Response.Text.Should().Contain($"{attributeValue}");
        }

        //ShowTop
        {
            var result = await Handle($"ёба покажи топ -10");
            result.Response.Text.Should().Contain($"{_profile.MainName}");
        }

        //ShowProfile
        {   var result = await Handle($"ёба покажи профиль {_profile.MainName}");
            result.Response.Text.Should().Contain($"{_profile.MainName}");
        }
    }


    async Task<Result<Message>> Handle(string text, string username = null)
    {
        var message = Setup.Message(text, username);
        var result = await _controller.Handle(message);
        result.Status.Should().Be(Status.Success);
        return (Result<Message>) result;
    }
}