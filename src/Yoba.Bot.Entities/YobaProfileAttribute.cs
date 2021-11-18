namespace Yoba.Bot.Entities;

public class YobaProfileAttribute
{
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; }
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; }
    public string Value { get; set; }
}