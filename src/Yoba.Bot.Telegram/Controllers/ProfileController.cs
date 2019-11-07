using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yoba.Bot.Telegram.Controllers
{
//    public class ProfileController : TelegramController
//    {
//        public ProfileController()
//        {
//            Add(new Regex(@"^(лойс|слив) (?<name>.+)$"),
//                m => new {name = m.Groups["name"].Value},
//                async (msg, context, cancel) =>
//                {
//                    await Task.Delay(32);
//                    Console.Write(context.name);
//                    return Result.Skip;
//                });
//        }
//    }
}
//    public class ProfileHandler : IHandler<Message>
//    {
//        readonly TelegramBotClient _telegram;
//
//        public ProfileHandler(TelegramBotClient telegram)
//        {
//            _telegram = telegram;
//        }
//
//        public async Task<Result<Message>> Handle(Request<Message> request, CancellationToken cancellation)
//        {
//            if (request.Message.Type != MessageType.Text)
//                return request.Skip();
//            Regex.Match("^$")
////            var replyProfile = await request.GetProperty<Profile>(ProfileMiddleware.ReplyToProfile, null, cancellation);
////            switch (request.Message.Text.ToLowerInvariant())
////            {
////                case "лойс" when replyProfile != null:
////                    
////            }
//        }
//    }
//
//    public class AddLoise
//    {
//        
//    }
//   
//}