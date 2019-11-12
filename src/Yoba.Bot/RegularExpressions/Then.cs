namespace Yoba.Bot.RegularExpressions
{
    public class Then : Re
    {
        readonly Re _a;
        readonly Re _b;

        internal Then(Re a, Re b)
        {
            _a = a;
            _b = b;
        }

        public override string ToString() => $"(?:{_a}{_b})";
    }
}