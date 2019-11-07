using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot
{
    public class SimpleAction<TMsg> : IHandler<TMsg>
    {
        readonly SimpleHandle<TMsg> _handle;

        public SimpleAction(SimpleHandle<TMsg> handle)
        {
            _handle = handle;
        }

        public Task<Result> Handle(Request<TMsg> request, CancellationToken cancel) => 
            _handle(request, cancel);
    }

    public delegate Task<Result> SimpleHandle<TMsg>(Request<TMsg> request, CancellationToken cancel);

}