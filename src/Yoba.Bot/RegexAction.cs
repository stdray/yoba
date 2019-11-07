using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class RegexAction<TMsg> : IHandler<TMsg>
    {
        readonly Regex _regex;
        readonly MatchHandle<TMsg> _handle;

        public RegexAction(Regex regex, MatchHandle<TMsg> handle)
        {
            _regex = regex;
            _handle = handle;
        }

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            try
            {
                var text = await request.GetProperty<string>(Property.Text, cancel: cancel);
                var match = _regex.Match(text);
                if (!match.Success)
                    return Result.Skip();
                return await _handle(request, match, cancel);
            }
            catch (Exception ex)
            {
                return Result.Error(ex);
            }
        }
    }
}