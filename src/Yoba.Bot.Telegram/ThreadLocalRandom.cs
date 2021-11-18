namespace Yoba.Bot.Telegram;

public class ThreadLocalRandom : IRandomGenerator
{
    static int _seed = Environment.TickCount;

    static readonly ThreadLocal<Random> Random =
        new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

    public int Next(int from, int to) => Random.Value.Next(from, to);
}