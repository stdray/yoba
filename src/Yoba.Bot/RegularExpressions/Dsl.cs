using System.Linq;
// ReSharper disable InconsistentNaming

namespace Yoba.Bot.RegularExpressions
{
    public static class Dsl
    {
        public static Re anyCh { get; } = new Const(".");
        public static Re begin { get; } = new Const("^");
        public static Re end { get; } = new Const("$");
        public static Re ws { get; } = new Const(@"\s");
        public static Re value(string value) => new Value(value);
        public static Re re(string value) => new Value(value);
        public static Re seq(Re re, params Re[] res) => res.Aggregate(re, (a, x) => a + x);
        public static Re anyOf(Re re, params Re[] res) => res.Aggregate(re, (a, x) => a | x);
        public static Re opt(this string v) => re(v).opt;
    }
}