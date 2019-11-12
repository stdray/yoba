namespace Yoba.Bot.Telegram
{
    public interface IRandomGenerator
    {
        int Next(int from, int to);
    }
}