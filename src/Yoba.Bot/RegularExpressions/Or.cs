namespace Yoba.Bot.RegularExpressions
{
    public class Or : Re
    {
        readonly Re _a;
        readonly Re _b;

        internal Or(Re a, Re b)
        {
            _a = a;
            _b = b;
        }

        public override string ToString() => $"(?:{_a}|{_b})";
    }
}