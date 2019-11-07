using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Yoba.Bot
{
    public static class ActionExtensions
    {
        public static void AddRegexRule<TMsg>(this Controller<TMsg> controller, Regex regex, MatchHandle<TMsg> handle)
        {
            var prov = controller.Providers.OfType<IProvider<TMsg, string>>().SingleOrDefault();
            if (prov == null)
                throw new KeyNotFoundException("Provider not found");
            controller.Add(new RegexAction<TMsg>(prov, regex, handle));
        }

        public static void AddSimpleRule<TMsg>(this Controller<TMsg> controller, SimpleHandle<TMsg> handle)
        {
            controller.Add(new SimpleAction<TMsg>(handle));
        }
    }
}