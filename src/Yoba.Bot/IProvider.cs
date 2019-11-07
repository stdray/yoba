using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public interface IProvider<in TMsg, TProp>
    {
        Task<TProp> Provide(string key, TMsg message,
            TProp defaultValue = default(TProp),
            CancellationToken cancellation = default(CancellationToken));
    }
}