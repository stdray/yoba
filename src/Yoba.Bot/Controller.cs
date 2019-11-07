using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class Controller<TMsg> : IController<TMsg>
    {
        public IReadOnlyCollection<IProvider<TMsg>> Providers { get; }
        readonly List<IHandler<TMsg>> _actions = new List<IHandler<TMsg>>();

        protected Controller(IEnumerable<IProvider<TMsg>> providers)
        {
            Providers = providers?.ToList() ?? new List<IProvider<TMsg>>(0);
        }

        public void Add(IHandler<TMsg> action) => _actions.Add(action);

        public async Task<Result> Handle(Request<TMsg> request, CancellationToken cancel)
        {
            var result = Result.Skip();
            foreach (var handler in _actions)
            {
                result = await handler.Handle(request, cancel);
                if ((result.Status & Status.Success) != 0)
                    break;
            }

            return result;
        }

        public Result Ok<T>(T response) => Result<T>.Success(response);
    }
}