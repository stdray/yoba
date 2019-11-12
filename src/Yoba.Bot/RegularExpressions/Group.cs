namespace Yoba.Bot.RegularExpressions
{
    public class Group : Re
    {
        readonly Re _re;
        readonly string _name;

        internal Group(Re re, string name = null)
        {
            _re = re;
            _name = name;
        }

        public override string ToString()
        {
            var p = string.IsNullOrEmpty(_name) ? "" : $"?<{_name}>";
            return $"({p}{_re})";
        }
    }
}