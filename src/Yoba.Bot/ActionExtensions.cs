using System.Text.RegularExpressions;
using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.RegularExpressions.Dsl;

namespace Yoba.Bot;

public static class ActionExtensions
{
    public static void AddRegexRule<TMsg>(this Controller<TMsg> controller, Regex regex, MatchHandle<TMsg> handle)
    {
        var prov = controller.Providers.OfType<IProvider<TMsg, string>>().SingleOrDefault();
        if (prov == null)
            throw new KeyNotFoundException("Provider not found");
        controller.Add(new RegexAction<TMsg>(prov, regex, handle));
    }

    public static void AddReRule<TMsg>(this Controller<TMsg> controller, Re re, MatchHandle<TMsg> handle,
        ReRuleOptions options = null)
    {
        options ??= new ReRuleOptions();
        if (options.ImplicitBeginEnd)
            re = begin + re + end;
        var regex = new Regex(re.ToString(), options.RegexOptions);
        AddRegexRule(controller, regex, handle);
    }


    public static void AddSimpleRule<TMsg>(this Controller<TMsg> controller, SimpleHandle<TMsg> handle)
    {
        controller.Add(new SimpleAction<TMsg>(handle));
    }
}