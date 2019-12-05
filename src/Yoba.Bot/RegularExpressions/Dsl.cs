using System.Linq;

// ReSharper disable InconsistentNaming

namespace Yoba.Bot.RegularExpressions
{
    public static class Dsl
    {
        public static Re phrase(string n) => anyCh.weakAny.group(n);
        public static Re anyCh { get; } = new Const(".");
        public static Re begin { get; } = new Const("^");
        public static Re end { get; } = new Const("$");
        public static Re space { get; } = new Const(@"\s");
        public static Re s { get; } = space.oneOrMore;
        public static Re digit { get; } = new Const(@"\d");
        public static Re w { get; } = new Const(@"\w");
        public static Re value(string value) => new Value(value);
        public static Re re(string value) => new Value(value);
        public static Re seq(Re re, params Re[] res) => res.Aggregate(re, (a, x) => a + x);
        public static Re anyOf(Re re, params Re[] res) => res.Aggregate(re, (a, x) => a | x);
        public static Re opt(this string v) => re(v).opt;
//        public static Re implicitSpaces(this Re re) => implicitSep(re, space.oneOrMore);
//
//        public static Re implicitSep(this Re re, Re sep)
//        {
//            if (!(re is Then then))
//                return re;
//            var head = then.Exprs.First();
//            var rest = then.Exprs.Skip(1);
//            return rest.Aggregate(head, (h, x) =>
//            {
//                if (x is Count count && count.IsOpt)
//                    return h + new Count(sep + count.Re, count.Min, count.Max, count.Weak);
//                return h + sep + x;
//            });
//        }
    }
}