namespace Yoba.Bot.RegularExpressions
{
    public abstract class Re
    {
        public abstract override string ToString();


        //public static implicit operator Regex(Re re) => new Regex(re.ToString());
        public static implicit operator Re(string re) => new Value(re);
        public static Re operator |(Re a, Re b) => new Or(a, b);
        public static Re operator |(Re a, string b) => new Or(a, new Value(b));
        public static Re operator +(Re a, Re b) => new Then(a, b);
        public static Re operator +(Re a, string b) => new Then(a, new Value(b));
        public Re this[int min, bool weak = false] => new Count(this, min, null, weak);
        public Re this[int min, int max, bool weak = false] => new Count(this, min, max, weak);
        public Re group(string name = null) => new Group(this, name);
        public Re opt => this[0, 1];
        public Re any => this[0];
        public Re weakAny => this[0, true];
        public Re oneOrMore => this[1];
                
    }
}