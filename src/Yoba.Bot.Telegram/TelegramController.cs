using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Yoba.Bot.Telegram
{
    public class TelegramController : Controller<Message>
    {
        public void Add<TCtx>(Regex regex, Parse<Message, TCtx> parse, Handle<TCtx> handle)
        {
            Add(new TelegramRegexAction<TCtx>(regex, parse, handle));
        }
    }

    public class TelegramRegexAction<TCtx> : RegexAction<Message, TCtx>
    {
        public TelegramRegexAction(Regex regex, Parse<Message, TCtx> parse, Handle<TCtx> handle)
            : base(regex, parse, handle)
        {
        }

        public override async Task<Result> Handle(Request<Message> request, CancellationToken cancel)
        {
            if (request.Message.Type != MessageType.Text)
                return Result.Skip;
            return await Handle(request, request.Message.Text, cancel);
        }
    }
}