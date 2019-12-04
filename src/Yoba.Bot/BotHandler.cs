using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class BotHandler<TMsg> : IHandler<TMsg>
    {
        readonly IEnumerable<IMiddleware<TMsg>> _middlewares;
        readonly IEnumerable<IController<TMsg>> _controllers;

        public BotHandler(IEnumerable<IController<TMsg>> controllers, IEnumerable<IMiddleware<TMsg>> middlewares = null)
        {
            _controllers = controllers;
            _middlewares ??= new List<IMiddleware<TMsg>>();
        }

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            var result = Result.Skip();
            foreach (var middleware in _middlewares)
            {
                await middleware.Execute(request, cancel);
            }

            foreach (var handler in _controllers)
            {
                result = await handler.Handle(request, cancel);
                if ((result.Status & Status.Success) == Status.Success)
                {
                    break;
                }
            }

            return result;
        }
    }
}