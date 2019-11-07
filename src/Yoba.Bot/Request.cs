using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class Request<TMsg>
    {
        readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public Request(TMsg message)
        {
            Message = message;
        }

        public TMsg Message { get; }

        public async Task<TProp> GetProperty<TProp>(string key,
            TProp defaultValue = default(TProp),
            CancellationToken cancellation = default(CancellationToken))
        {
            if (!_properties.TryGetValue(key, out var value))
                return defaultValue;
            if (value is IProvider<TMsg, TProp> provider)
                return await provider.Provide(key, Message, defaultValue, cancellation);
            return (TProp) value;
        }

        public void AddProperty<TProp>(string key, TProp value)
        {
            _properties.Add(key, value);
        }
    }
}