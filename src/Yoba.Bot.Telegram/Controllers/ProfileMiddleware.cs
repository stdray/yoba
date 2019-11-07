namespace Yoba.Bot.Telegram.Controllers
{
//    public class ProfileMiddleware : IMiddleware<Message>
//    {
//        public const string FromProfile = "FromProfile";
//        public const string ReplyToProfile = "ReplyToProfile";
//
//        readonly ProfileProvider _fromProvider;
//        readonly ProfileProvider _replyToProvider;
//
//        public ProfileMiddleware(IFactory<YobaBotDB> dbFactory)
//        {
//            _fromProvider = new ProfileProvider(dbFactory, x => x.From.Username);
//            _replyToProvider = new ProfileProvider(dbFactory, x => x.ReplyToMessage.From.Username);
//        }
//
//        public Task Execute(Request<Message> message, CancellationToken cancellation = default(CancellationToken))
//        {
//            message.AddProperty(FromProfile, _fromProvider);
//            if (message.Message.ReplyToMessage != null)
//                message.AddProperty(ReplyToProfile, _replyToProvider);
//            return Task.CompletedTask;
//        }
//    }
}