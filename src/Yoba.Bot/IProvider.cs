using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public interface IProvider<TMsg>
    {
        
    }
    
    public interface IProvider<TMsg, TProp> : IProvider<TMsg>
    {
        Task<TProp> Provide(TMsg message,
            TProp defaultValue = default(TProp),
            CancellationToken cancellation = default(CancellationToken));
    }
}