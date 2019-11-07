using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public interface IHandler<TMsg>
    {
        Task<Result> Handle(Request<TMsg> request, CancellationToken cancel);
    }
}