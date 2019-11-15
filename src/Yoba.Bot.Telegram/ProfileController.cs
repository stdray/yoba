using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.Entities;
using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.RegularExpressions.Dsl;
using static Yoba.Bot.Telegram.Shared;

// ReSharper disable InconsistentNaming

namespace Yoba.Bot.Telegram
{
    public class ProfileController : Controller<Message>
    {
        readonly ITelegramBotClient _telegram;
        readonly IProfileDao _dao;

        public ProfileController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram, IProfileDao dao)
            : base(providers)
        {
            _telegram = telegram;
            _dao = dao;

            AddProfile();
            AddProfileAlias();
            AddProfileLois();
            AddProfileSliv();
            AddProfileZashkvor();
            SetProfileAttr();
            CreateAttr();
            ShowAttributeValues();
            ListAttributes();
            ShowTop();
            ShowProfile();
        }

        static Re name(string groupName) => w.oneOrMore.group(groupName);
        static readonly Re s_For_s = s.oneOrMore + "для" + s.oneOrMore;
        readonly Re profile = anyOf("профиль") + s.oneOrMore;
        readonly Re attribute = anyOf("атрибут", "аттрибут", "параметер", "параметр", "значение", "инфо");

        void AddProfile()
        {
            this.AddReRule(
                bot + add + profile + name("name"),
                async (request, match, cancel) =>
                {
                    await _dao.CreateProfile(new YobaProfile
                    {
                        Id = Guid.NewGuid(),
                        MainName = match.Value("name"),
                    }, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Профиль создан", cancel));
                });
        }

        void AddProfileAlias()
        {
            var alias = anyOf("алиас", "alias", "псевдоним", "погоняло", "кличку", "позывной");
            this.AddReRule(
                bot + add.opt + alias + name("alias") + s_For_s + name("name"),
                async (request, match, cancel) =>
                {
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    await _dao.AddAlias(to.Id, match.Value("alias"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }

        void AddProfileLois()
        {
            var lois = anyOf("лойс", "лимон", "лаймон", "апельсин", "lois", "лайк", "нойс");
            this.AddReRule(
                (bot.opt + lois + name("name")) | (name("name") + s.oneOrMore + lois),
                async (request, match, cancel) =>
                {
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    await _dao.AddLois(to.Id, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Лойс поставлен", cancel));
                });
        }

        void AddProfileSliv()
        {
            var sliv = anyOf("слив");
            this.AddReRule(
                (bot.opt + sliv + name("name")) | (name("name") + s.oneOrMore + sliv),
                async (request, match, cancel) =>
                {
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    await _dao.AddLois(to.Id, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Слит", cancel));
                });
        }

        void AddProfileZashkvor()
        {
            var zashkvor = anyOf("зашквор", "зашквар", "zashkvor", "zashkvar");
            this.AddReRule(
                (bot.opt + zashkvor + name("name")) | (name("name") + s.oneOrMore + zashkvor),
                async (request, match, cancel) =>
                {
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    await _dao.AddZashkvor(to.Id, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Зашквор засчитан", cancel));
                });
        }

        void ShowProfile()
        {
            this.AddReRule(
                bot + show + profile + name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void ShowTop()
        {
            double karma(YobaProfile p)
            {
                var k = Math.Sqrt(Math.Pow(p.Loisy, 2) + Math.Pow(p.Zashkvory, 2)) / 100500.0;
                return k * (p.Loisy - Math.Sqrt(0.93 * Math.Pow(p.Zashkvory, 2)));
            }
            this.AddReRule(
                bot + show + "топ" + s.oneOrMore + ("-".opt() + d.oneOrMore).group("count") +
                (s.oneOrMore + "пользователей").opt,
                async (request, match, cancel) =>
                {
                    var profs = await _dao.GetProfiles(cancel);
                    var count = int.Parse(match.Value("count"));
                    var lines = (count < 0 ? profs.OrderBy(karma) : profs.OrderByDescending(karma))
                        .Take(Math.Abs(count))
                        .Select(x => $"{x.Names} [Лойсы: {x.Loisy}; Зашкворы: {x.Zashkvory}; Сливы: {x.Slivi}]");
                    var text = string.Join(Environment.NewLine, lines);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void ListAttributes()
        {
            this.AddReRule(
                bot + show + "список" + s.oneOrMore + "атрибутов",
                async (request, match, cancel) =>
                {
                    var attributes = await _dao.GetAttributes(cancel);
                    var text = string.Join(Environment.NewLine, attributes.Select(a => a.Name));
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void ShowAttributeValues()
        {
            this.AddReRule(
                bot + show + attribute + s.oneOrMore + name("name"),
                async (request, match, cancel) =>
                {
                    var values = await _dao.GetProfileAttributes(match.Value("name"), cancel);
                    var lines = values.Select(a => $"{a.ProfileName}: {a.Value}");
                    var text = string.Join(Environment.NewLine, lines);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void CreateAttr()
        {
            this.AddReRule(
                bot + create + attribute + s.oneOrMore + name("name"),
                async (request, match, cancel) =>
                {
                    await _dao.CreateAttribute(new YobaAttribute
                    {
                        Id = Guid.NewGuid(),
                        Name = match.Value("name")
                    }, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }

        void SetProfileAttr()
        {
            this.AddReRule(
                bot + attribute + name("attr") + s.oneOrMore + name("name") + s.any + ":" +
                anyCh.oneOrMore.@group("content"),
                async (request, match, cancel) =>
                {
                    var attr = await _dao.FindAttribute(match.Value("attr"), cancel);
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    await _dao.SetProfileAttribute(new YobaProfileAttribute
                    {
                        Value = match.Value("content"),
                        AttributeId = attr.Id,
                        ProfileId = prof.Id,
                    }, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }
    }
}