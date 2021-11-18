using System.Text.RegularExpressions;

namespace Yoba.Bot.RegularExpressions;

public class Value : Re
{
    readonly string _raw;

    internal Value(string raw)
    {
        _raw = raw;
    }

    public override string ToString() => Regex.Escape(_raw);
}