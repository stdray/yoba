using System;

namespace Yoba.Bot.Entities
{
    public class YobaProfileAttribute
    {
        public YobaAttribute Attribute { get; set; }
        public Guid ProfileId { get; set; }
        public string Value { get; set; }
    }
}