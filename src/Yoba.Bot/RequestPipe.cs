//using System.Threading;
//using System.Threading.Tasks;
//
//namespace Yoba.Bot
//{
//    public abstract class RequestPipe<TMsg>
//    {
////        readonly MiddlewareCollection<TMsg> _middlewareCollection;
//        readonly HandlerCollection<TMsg> _handlerCollection;
//
//        protected RequestPipe(HandlerCollection<TMsg> handlerCollection
////            MiddlewareCollection<TMsg> middlewareCollection = null
//            )
//        {
//            _handlerCollection = handlerCollection;
////            _middlewareCollection = middlewareCollection ?? new MiddlewareCollection<TMsg>();
//        }
//
//        public async Task Handle(Request<TMsg> request, CancellationToken cancellation)
//        {
//            foreach (var handler in _handlerCollection)
//                if (await handler.Handle(request, cancellation))
//                    break;
//        }
//    }
//}