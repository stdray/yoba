using System;
using System.Collections.Generic;

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
    }
}