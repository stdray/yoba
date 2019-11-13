using System;

namespace Yoba.Bot.Entities
{
    public class YobaNote
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}