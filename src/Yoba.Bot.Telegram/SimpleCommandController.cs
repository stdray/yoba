using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using static Yoba.Bot.Telegram.Shared;

namespace Yoba.Bot.Telegram
{
    public class SimpleCommandController : Controller<Message>
    {
        readonly ITelegramBotClient _telegram;

        public SimpleCommandController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram) 
            : base(providers)
        {
            _telegram = telegram;

            Ping();
            Version();
//            Vanga();
        }

//        void Vanga()
//        {
//            const string answers = @"(?<answers>(.+?)(?:,|или|$)";
//            const string question = @"(?<question>[^:]*)?";
//            var pattern = $@"{BotName}(вангуй)\s+{question}\s*{answers}";
//            this.AddRegexRule(
//                new Regex(pattern), 
//                async (request, match, cancel) =>
//                {
//                    var answers = match.Groups["answers"].Value
//                        .Split(new[] {",", " или "}, StringSplitOptions.RemoveEmptyEntries)
//                        .Select(x => x.Trim())
//                        .ToList();
//                    
//                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
//                });
//        }

        void Version() => this.AddRegexRule(
            new Regex($@"^\s*{BotName}\s*(version|версия)\s*$"),
            async (request, _, cancel) =>
            {
                var text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return Ok(await _telegram.ReplyAsync(request, text, cancel));
            });

        void Ping() => this.AddRegexRule(
            new Regex(@"^\s*(?<cmd>ping|пинг|1)\s*$"),
            async (request, match, cancel) =>
            {
                var cmd = match.Groups["cmd"].Value.Trim();
                var text = cmd == "1" ? "1" : "pong";
                return Ok(await _telegram.ReplyAsync(request, text, cancel));
            });
    }
}