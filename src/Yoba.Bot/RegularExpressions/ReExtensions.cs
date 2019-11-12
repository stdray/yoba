using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Yoba.Bot.RegularExpressions
{
    public static class ReExtensions
    {
        public static IEnumerable<string> Values(this Match match, int number) => 
            Clean(match.Groups[number].Captures);

        public static IEnumerable<string> Values(this Match match, string name) => 
            Clean(match.Groups[name].Captures);

        public static IEnumerable<string> Clean(CaptureCollection captures)
        {
            foreach (Capture capture in captures)
            {
                var v = capture.Value.Trim();
                if (!string.IsNullOrEmpty(v))
                    yield return v;
            }
        }
    }
}