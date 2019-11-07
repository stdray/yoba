using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public interface IHandler<TMsg>
    {
        Task<Result> Handle(Request<TMsg> request, CancellationToken cancel);
    }

    public class Controller<TMsg> : IHandler<TMsg>
    {
        readonly List<IHandler<TMsg>> _actions = new List<IHandler<TMsg>>();

        public void Add(IHandler<TMsg> action) => _actions.Add(action);

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            var result = Result.Skip;
            foreach (var handler in _actions)
            {
                result = await handler.Handle(request, cancel);
                if ((result.Status & HandleStatus.Stop) != 0)
                    break;
            }

            return result;
        }
    }


    public delegate Task<Result> Handle<in TCtx>(TCtx context, CancellationToken cancel = default(CancellationToken));

    public delegate TCtx Parse<TMsg, out TCtx>(Request<TMsg> message, Match match);

    public abstract class RegexAction<TMsg, TCtx> : IHandler<TMsg>
    {
        readonly Regex _regex;
        readonly Parse<TMsg, TCtx> _parse;
        readonly Handle<TCtx> _handle;

        protected RegexAction(Regex regex, Parse<TMsg, TCtx>  parse, Handle<TCtx> handle)
        {
            _regex = regex;
            _parse = parse;
            _handle = handle;
        }

        public async Task<Result> Handle(Request<TMsg> msg, string input, CancellationToken cancel)
        {
            try
            {
                var match = _regex.Match(input);
                var ctx = _parse(msg, match);
                var res = await _handle(ctx, cancel);
                return res;
            }
            catch (Exception ex)
            {
                return Result.Error(ex);
            }
        }

        public abstract Task<Result> Handle(Request<TMsg> request, CancellationToken cancel);
    }

//    public static class MatchExtensions
//    {
//        public static string Str(this Match match, string key, bool required = true, string @default = null)
//        {
//            var group = match.Groups[key];
//            if (group.Success)
//                return group.Value.Trim();
//            if (required)
//                throw new KeyNotFoundException(key);
//            return @default;
//        }
//        
//        public 
//    }
}