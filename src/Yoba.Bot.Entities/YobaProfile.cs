using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoba.Bot.Entities
{
    public class YobaProfile
    {
        public Guid Id { get; set; }
        public string MainName { get; set; }
        public int Loisy { get; set; }
        public int Zashkvory { get; set; }
        public int Slivi { get; set; }
        public bool CanVote { get; set; }
        public IReadOnlyCollection<string> Names { get; set; }
        public IReadOnlyCollection<YobaProfileAttribute> Attributes { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder().AppendLine(MainName);
            if (Names?.Any() == true)
                sb.AppendLine(string.Join(", ", Names));
            sb.AppendLine($"Лойсы: {Loisy}")
                .AppendLine($"Зашкворы: {Zashkvory}")
                .AppendLine($"Сливы: {Slivi}")
                .AppendLine($"Может голосовать: {CanVote}");
            foreach (var attribute in Attributes)
                sb.AppendLine($"{attribute.AttributeName}: {attribute.Value}");
            return sb.ToString();
        }
    }
}