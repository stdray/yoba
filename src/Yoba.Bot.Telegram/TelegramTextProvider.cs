using Telegram.Bot.Types;

namespace Yoba.Bot.Telegram;

public class TelegramTextProvider : IProvider<Message, string>
{
    public Task<string> Provide(Message message, string defaultValue = default,
        CancellationToken cancellation = default)
    {
        return Task.FromResult(message.Text);
    }
}