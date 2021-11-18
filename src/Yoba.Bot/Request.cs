namespace Yoba.Bot;

public class Request<TMsg>
{
    readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        
    public Request(TMsg message)
    {
        Message = message;
    }

    public TMsg Message { get; }

    public TProp GetProperty<TProp>(string key, TProp @default = default)
    {
        return !_properties.TryGetValue(key, out var value) ? @default : (TProp) value;
    }

    public void AddProperty<TProp>(string key, TProp value)
    {
        _properties.Add(key, value);
    }
}