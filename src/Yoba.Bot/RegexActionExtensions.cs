using System.Text.RegularExpressions;

namespace Yoba.Bot
{
    public static class ActionExtensions
    {
        public static void AddRegexRule<TMsg>(this Controller<TMsg> controller, Regex regex, MatchHandle<TMsg> handle)
        {
            controller.Add(new RegexAction<TMsg>(regex, handle));
        }

        public static void AddSimpleRule<TMsg>(this Controller<TMsg> controller, SimpleHandle<TMsg> handle)
        {
            controller.Add(new SimpleAction<TMsg>(handle));
        }
    }
}