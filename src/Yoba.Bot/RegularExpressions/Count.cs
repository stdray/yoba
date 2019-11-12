namespace Yoba.Bot.RegularExpressions
{
    public class Count : Re
    {
        readonly Re _re;
        readonly int _min;
        readonly int? _max;
        readonly bool _weak;

        internal Count(Re re, int min, int? max, bool weak)
        {
            _re = re;
            _min = min;
            _max = max;
            _weak = weak;
        }

        public override string ToString()
        {
            var max = _max.HasValue ? _max.ToString() : "";
            var weak = _weak ? "?" : "";
            return $"(?:{_re}){{{_min},{max}}}{weak}";
        }
    }
}