using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public abstract class RequestPipe<TMsg>
    {
        readonly IEnumerable<IMiddleware<TMsg>> _middlewares;
        readonly IEnumerable<IHandler<TMsg>> _handlers;

        protected RequestPipe(
            IEnumerable<IHandler<TMsg>> handlers,
            IEnumerable<IMiddleware<TMsg>> middlewares = null)
        {
            _handlers = handlers;
            _middlewares = _middlewares ?? new List<IMiddleware<TMsg>>();
        }

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            var result = Result.Skip();
            foreach (var middleware in _middlewares)
            {
                await middleware.Execute(request, cancel);
            }
            foreach (var handler in _handlers)
            {
                result = await handler.Handle(request, cancel);
                if ((result.Status & Status.Success) != 0)
                    break;
            }
            return result;
        }
    }
}