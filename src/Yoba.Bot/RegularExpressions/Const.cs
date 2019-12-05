namespace Yoba.Bot.RegularExpressions
{
    public class Const : Re
    {
        readonly string _raw;

        public Const(string raw)
        {
            _raw = raw;
        }

        public override string ToString() => _raw;
    }
}