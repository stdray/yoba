using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BinaryAnalysis.UnidecodeSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.Telegram.Shared;
using static Yoba.Bot.RegularExpressions.Dsl;

// ReSharper disable InconsistentNaming

namespace Yoba.Bot.Telegram
{
    public class SimpleController : Controller<Message>
    {
        readonly ITelegramBotClient _telegram;
        readonly IRandomGenerator _random;

        public SimpleController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram,
            IRandomGenerator random) : base(providers)
        {
            _telegram = telegram;
            _random = random;

            Ping();
            Version();
            Vanga();
            Translit();
        }

        void Translit() => this.AddReRule(
            bot + anyOf("транслитом", "translit") + anyCh.oneOrMore.group("text"),
            async (request, match, cancel) =>
            {
                var text = match.Values("text").Single();
                return Ok(await _telegram.ReplyAsync(request, text.Unidecode(), cancel));
            });

        void Vanga()
        {
            const string name = "answers";
            Re phrase(string n) => anyCh.weakAny.group(n);
            var sp = s + "или" + s | ",";
            var vanga = anyOf("вангуй", "гадай");
            var question = phrase(string.Empty);
            var answers = s.opt + phrase(name) + (sp + phrase(name)).oneOrMore;

            async Task<Result> Reply(Request<Message> request, Match match, CancellationToken cancel)
            {
                var defaultAnswers = new[]
                {
                    "да", "нет", "это не важно", "спок, бро", "толсто",
                    "да, хотя зря", "никогда", "100%", "1 из 100"
                };
                var choices = match.Values(name).ToArray();
                choices = choices.Any() ? choices : defaultAnswers;
                var choice = choices[_random.Next(0, choices.Length)];
                return Ok(await _telegram.ReplyAsync(request, choice, cancel));
            }

            this.AddReRule(bot + vanga + s + question + s.opt + ":" + answers + "?".opt(), Reply);
            this.AddReRule(bot + vanga + s + answers + "?".opt(), Reply);
            this.AddReRule(bot + vanga + s + question + "?".opt(), Reply);
        }

        void Version() => this.AddReRule(
            bot + anyOf("версия", "version"),
            async (request, _, cancel) =>
            {
                var text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return Ok(await _telegram.ReplyAsync(request, text, cancel));
            });

        void Ping() => this.AddReRule(
            anyOf("ping", "пинг", "1").group("cmd"),
            async (request, match, cancel) =>
            {
                var cmd = match.Groups["cmd"].Value.Trim();
                var text = cmd == "1" ? "1" : "pong";
                return Ok(await _telegram.ReplyAsync(request, text, cancel));
            });
    }
}