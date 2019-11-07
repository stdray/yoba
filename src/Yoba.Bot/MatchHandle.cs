using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public delegate Task<Result> MatchHandle<TMsg>(Request<TMsg> request, Match match, CancellationToken cancel);
}