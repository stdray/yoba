using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.RegularExpressions.Dsl;

namespace Yoba.Bot.Telegram
{
    public static class Shared
    {
        public static Re bot { get; } = anyOf("yoba", "ёба", "ёбамысо") + ws.oneOrMore;
    }
}