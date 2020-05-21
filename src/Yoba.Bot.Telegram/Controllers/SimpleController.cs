using System;
using System.Collections.Generic;
using System.IO;
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
using File = System.IO.File;

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
            Help();
        }

        void Help()
        {
            this.AddReRule(
                bot + s + anyOf("help", "справка", "?", "-h"),
                async (request, match, cancel) =>
                {
                    var dir = AppDomain.CurrentDomain.BaseDirectory;
                    var file = Path.Combine(dir, "YobaBotHelp.txt");
                    var text = File.ReadAllText(file);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void Translit()
        {
            var translit = anyOf("транслитом", "translit");
            this.AddReRule(
                bot + s + translit + s + phrase("text"),
                async (request, match, cancel) =>
                {
                    var text = match.Values("text").Single();
                    return Ok(await _telegram.ReplyAsync(request, text.Unidecode(), cancel));
                });
        }

        void Vanga()
        {
            const string name = "answers";
            var sp = s + "или" + s | ",";
            var vanga = anyOf("вангуй", "гадай");
            var question = phrase("question");
            var answers = phrase(name) + (sp + phrase(name)).oneOrMore;

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

            this.AddReRule(bot + s + vanga + s + question + space.any + ":" + answers + "?".opt(), Reply);
            this.AddReRule(bot + s + vanga + s + answers + "?".opt(), Reply);
            this.AddReRule(bot + s + vanga + s + question + "?".opt(), Reply);
        }

        void Version() => this.AddReRule(
            bot + s + anyOf("версия", "version"),
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