using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static Yoba.Bot.Telegram.Controllers.Shared;

namespace Yoba.Bot.Telegram.Controllers
{
    public class SimpleCommandController : Controller<Message>
    {
        readonly ITelegramBotClient _telegram;

        public SimpleCommandController(ITelegramBotClient telegram)
        {
            _telegram = telegram;
            
            Ping();
            Version();
        }

        void SetUpActions()
        {

//            const string answers = @"(?<answers>(.+?)(?:,|$)";
//            const string question = @"(?<question>[^:]*)";
//            Add(new Regex($@"^\s*{BotName}(вангуй)\s+\s*$"),
//                (m => m.Groups["answers"].);
        }

        void Vanga()
        {
            const string answers = @"(?<answers>(.+?)(?:,|или|$)";
            const string question = @"(?<question>[^:]*)?";
            var pattern = $@"{BotName}(вангуй)\s+{question}\s*{answers}";
            this.AddRegexRule(
                new Regex(pattern), 
                async (request, match, cancel) =>
                {
                    var answers = match.Groups["answers"].Value
                        .Split(new[] {",", " или "}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                    
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }
        
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

    public static class Shared
    {
        public const string BotName = @"\s*(?<botName>(yoba|ёба|еба|ёбамысо))\s+";
    }

    public static class TelegramBotClientExtensions
    {
        public static Task<Message> ReplyAsync(this ITelegramBotClient client, Request<Message> msg, string text,
            CancellationToken cancel) => client.SendTextMessageAsync(
            msg.Message.Chat.Id,
            text,
            replyToMessageId: msg.Message.MessageId,
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: cancel);
    }
}