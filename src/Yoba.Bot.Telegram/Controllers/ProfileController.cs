using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public ProfileController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram,
            IProfileDao dao) : base(providers)
        {
            _telegram = telegram;
            _dao = dao;
            
            NewAttribute();
            NewProfile();
            AddProfileAlias();
            AddProfileKarma();
            SetProfileAttributeValue();
            ListAttributes();
            ShowAttributeValues();
            ShowTop();
            ShowProfile();
        }

        static Re name(string groupName) => w.oneOrMore.group(groupName);
        readonly Re profile = anyOf("профиль");
        readonly Re attribute = anyOf("атрибут", "аттрибут", "параметер", "параметр", "значение", "инфо");

        void NewProfile()
        {
            this.AddReRule(
                bot + s + add + s + profile + s + phrase("name"),
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
                bot + (s + add).opt + s + alias + s + phrase("alias") + s + "для" + s + phrase("name"),
                async (request, match, cancel) =>
                {
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    if(to == null)
                        return Ok(await _telegram.ReplyAsync(request, "Профиль не найден", cancel));
                    var name = match.Value("alias").Trim();
                    await _dao.AddAlias(to.Id, name, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }

        void AddProfileKarma()
        {
            Re MakeRule(Re cmd) => ((bot + s).opt + cmd + s + phrase("name")) | (phrase("name") + s + cmd);

            MatchHandle<Message> MakeHandle(Func<Guid, CancellationToken, Task> upd, string text) =>
                async (request, match, cancel) =>
                {
                    var from = await _dao.FindProfile(request.Message.From.Username, cancel);
                    if(from == null || !from.CanVote)
                        return Ok(await _telegram.ReplyAsync(request, "Вы не можете голосовать", cancel));
                    var to = await _dao.FindProfile(match.Value("name"), cancel);
                    if(from.Id == to.Id)
                        return Ok(await _telegram.ReplyAsync(request, "Нельзя менять карму самому себе", cancel));
                    await upd(to.Id, cancel);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                };

            this.AddReRule(
                MakeRule(anyOf("лойс", "лимон", "лаймон", "апельсин", "lois", "лайк", "нойс")),
                MakeHandle(_dao.AddLois, "Лойс поставлен"));
            this.AddReRule(
                MakeRule(anyOf("слив")),
                MakeHandle(_dao.AddSliv, "Слит"));
            this.AddReRule(
                MakeRule(anyOf("зашквор", "зашквар", "zashkvor", "zashkvar")),
                MakeHandle(_dao.AddZashkvor, "Зашквор засчитан"));
        }

        void ShowProfile()
        {
            this.AddReRule(
                bot + s + show + s + profile + s + phrase("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void ShowTop()
        {
            static double karma(YobaProfile p)
            {
                var k = Math.Sqrt(Math.Pow(p.Loisy, 2) + Math.Pow(p.Zashkvory, 2)) / 100500.0;
                return k * (p.Loisy - Math.Sqrt(0.93 * Math.Pow(p.Zashkvory, 2)));
            }

            var users = anyOf("пользователей", "юзеров", "батманов");
            this.AddReRule(
                bot + s + show + s + "топ" + s + ("-".opt() + digit.oneOrMore).group("count") + (s + users).opt,
                async (request, match, cancel) =>
                {
                    var profs = await _dao.GetProfiles(cancel);
                    var count = int.Parse(match.Value("count"));
                    var lines = (count < 0 ? profs.OrderBy(karma) : profs.OrderByDescending(karma))
                        .Take(Math.Abs(count))
                        .Select(x => $"{x.MainName} [Лойсы: {x.Loisy}; Зашкворы: {x.Zashkvory}; Сливы: {x.Slivi}]");
                    var text = string.Join(Environment.NewLine, lines);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void ListAttributes()
        {
            this.AddReRule(
                bot + s + show + s + "список" + s + "атрибутов",
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
                bot + s + show + s + attribute + s + name("name"),
                async (request, match, cancel) =>
                {
                    var values = await _dao.GetProfileAttributes(match.Value("name"), cancel);
                    var lines = values.Select(a => $"{a.ProfileName}: {a.Value}");
                    var text = string.Join(Environment.NewLine, lines);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void NewAttribute()
        {
            this.AddReRule(
                bot + s + add + s + attribute + s + name("name"),
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

        void SetProfileAttributeValue()
        {
            this.AddReRule(
                bot + s + attribute + s + name("attr") + s + "для" + s + phrase("name") + space.any + ":" + phrase("data"),
                async (request, match, cancel) =>
                {
                    var attr = await _dao.FindAttribute(match.Value("attr"), cancel);
                    if(attr == null)
                        return Ok(await _telegram.ReplyAsync(request, "Атрибут не найден", cancel));
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    if(prof == null)
                        return Ok(await _telegram.ReplyAsync(request, "Профиль не найден", cancel));
                    await _dao.SetProfileAttribute(new YobaProfileAttribute
                    {
                        Value = match.Value("data"),
                        AttributeId = attr.Id,
                        ProfileId = prof.Id,
                    }, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }
    }
}