using System.Collections.Generic;

namespace Yoba.Bot.RegularExpressions
{
    public class Then : Re
    {
        public IReadOnlyCollection<Re> Exprs { get; }

        internal Then(Re a, Re b)
        {
            var exprs = new List<Re>();
            Add(exprs, a);
            Add(exprs, b);
            Exprs = exprs;
        }

        static void Add(List<Re> exprs, Re re)
        {
            if (re is Then then)
                exprs.AddRange(then.Exprs);
            else
                exprs.Add(re);
        }

        public override string ToString() => string.Join("", Exprs);
    }
}