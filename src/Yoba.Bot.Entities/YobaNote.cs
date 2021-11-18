namespace Yoba.Bot.Entities;

public class YobaNote
{
    string _displayName;

    public string DisplayName
    {
        get => _displayName;
        set
        {
            _displayName = value;
            Name = MakePkName(value);
        }
    }

    public string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Name { get; set; }

    public static string MakePkName(string name) =>
        name?.Trim().ToLower().Replace(" ", "");
}