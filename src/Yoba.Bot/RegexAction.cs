using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class RegexAction<TMsg> : IHandler<TMsg>
    {
        readonly IProvider<TMsg, string> _textProvider;
        readonly Regex _regex;
        readonly MatchHandle<TMsg> _handle;

        public RegexAction(IProvider<TMsg, string> textProvider, Regex regex, MatchHandle<TMsg> handle)
        {
            _textProvider = textProvider;
            _regex = regex;
            _handle = handle;
        }

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            try
            {
                var text = await _textProvider.Provide(request.Message, null, cancel);
                if (string.IsNullOrEmpty(text))
                    return Result.Skip();
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