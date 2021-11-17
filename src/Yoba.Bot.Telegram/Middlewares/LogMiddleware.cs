using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Yoba.Bot.Telegram.Middlewares
{
    public class LogMiddleware : IMiddleware<Message>
    {
        readonly ILogger<LogMiddleware> _logger;
        readonly Config _config;

        public LogMiddleware(ILogger<LogMiddleware> logger, Config config = null)
        {
            _logger = logger;
            _config = config ?? new Config();
        }

        public Task BeforeHandle(Request<Message> message, CancellationToken cancel = default) =>
            Task.CompletedTask;

        public Task AfterHandle(Request<Message> message, Result result, CancellationToken cancel = default)
        {
            var frm = message.Message.From.Username;
            var req = Substring(message.Message.Text, _config.MaxRequestLogLength);
            if (result.Status == Status.Success)
            {
                var rsp = result is Result<Message> r
                    ? Substring(r.Response.Text, _config.MaxResponseLogLength)
                    : "Ok";
                _logger.LogInformation("{from}: {request} => {response}", frm, req, rsp);
            }
            else if (result.Status == Status.Fail)
            {
                _logger.LogError(result.Exception, "{from}: {request} => Error", frm, req);
            }
            return Task.CompletedTask;
        }

        string Substring(string str, int maxLen) =>
            str==null ? string.Empty : str.Substring(0, Math.Min(maxLen, str.Length));

        public class Config
        {
            public int MaxRequestLogLength { get; set; } = 280;
            public int MaxResponseLogLength { get; set; } = 280;
        }
    }
}