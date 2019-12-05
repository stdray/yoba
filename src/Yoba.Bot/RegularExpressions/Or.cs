using System.Collections.Generic;

namespace Yoba.Bot.RegularExpressions
{
    public class Or : Re
    {
       IReadOnlyCollection<Re> Exprs { get; }

        internal Or(Re a, Re b)
        {
            var exprs = new List<Re>();
            Add(exprs, a);
            Add(exprs, b);
            Exprs = exprs;
        }

        static void Add(List<Re> exprs, Re re)
        {
            if (re is Or or)
                exprs.AddRange(or.Exprs);
            else
                exprs.Add(re);
        }


        public override string ToString() => "(?:" + string.Join("|", Exprs) + ")";
    }
}