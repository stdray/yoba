using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public interface IMiddleware<TMsg>
    {
        Task BeforeHandle(Request<TMsg> message, CancellationToken cancel = default);
        Task AfterHandle(Request<TMsg> message, Result result, CancellationToken cancellation = default);
    }
}