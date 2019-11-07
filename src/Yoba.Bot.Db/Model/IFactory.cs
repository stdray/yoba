namespace Yoba.Bot.Db
{
    public interface IFactory<out T>
    {
        T Create();
    }
}