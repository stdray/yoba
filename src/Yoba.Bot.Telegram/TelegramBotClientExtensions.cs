using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yoba.Bot.Telegram;

public static class TelegramBotClientExtensions
{
    public static Task<Message> ReplyAsync(this ITelegramBotClient client, Request<Message> msg, string text,
        CancellationToken cancel) => client.SendTextMessageAsync(
        msg.Message.Chat.Id,
        text,
        replyToMessageId: msg.Message.MessageId,
        cancellationToken: cancel);
}