using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public delegate Task<Result> SimpleHandle<TMsg>(Request<TMsg> request, CancellationToken cancel);
}