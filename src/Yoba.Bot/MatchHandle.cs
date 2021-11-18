using System.Text.RegularExpressions;

namespace Yoba.Bot;

public delegate Task<Result> MatchHandle<TMsg>(Request<TMsg> request, Match match, CancellationToken cancel);