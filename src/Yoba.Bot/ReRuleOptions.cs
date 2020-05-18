using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.RegexOptions;

namespace Yoba.Bot
{
    public class ReRuleOptions
    {
        public bool ImplicitBeginEnd { get; set; } = true;

        public RegexOptions RegexOptions { get; set; } = Singleline | IgnoreCase | Compiled;
    }
}