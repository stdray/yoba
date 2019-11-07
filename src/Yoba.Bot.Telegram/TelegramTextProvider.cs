using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Yoba.Bot.Telegram
{
    public class TelegramTextProvider : IProvider<Message, string>
    {
        public Task<string> Provide(Message message, string defaultValue = default(string),
            CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult(message.Text);
        }
    }
}