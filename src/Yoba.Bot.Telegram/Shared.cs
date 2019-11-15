using System.Text.RegularExpressions;
using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.RegularExpressions.Dsl;

namespace Yoba.Bot.Telegram
{
    public static class Shared
    {
        public static Re bot { get; } = anyOf("yoba", "ёба", "ёбамысо") + s.oneOrMore;
        public static Re show { get; } = anyOf("покажи") + s.oneOrMore;
        public static Re add { get; } = anyOf("добавь", "show") + s.oneOrMore;
        public static Re create { get; } = anyOf("добавь", "создай")  + s.oneOrMore;
    }
}
