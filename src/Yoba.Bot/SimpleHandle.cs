namespace Yoba.Bot;

public delegate Task<Result> SimpleHandle<TMsg>(Request<TMsg> request, CancellationToken cancel);