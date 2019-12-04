using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Yoba.Bot.Telegram
{
    public class TelegramTextProvider : IProvider<Message, string>
    {
        public Task<string> Provide(Message message, string defaultValue = default,
            CancellationToken cancellation = default)
        {
            var text = message.ForwardFrom != null || message.ForwardFromChat != null || message.From.IsBot
                ? defaultValue
                : message.Text;
            return Task.FromResult(text);
        }
    }
}