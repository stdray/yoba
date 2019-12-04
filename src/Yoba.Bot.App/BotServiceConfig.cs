using Yoba.Bot.Telegram;

namespace Yoba.Bot.App
{
    public class BotServiceConfig
    {
        public string TelegramToken { get; set; }
        public long GroupChatId { get; set; }
        public Socks5ProxyConfig Proxy { get; set; }
        public string ConnectionString { get; set; }
    }
}