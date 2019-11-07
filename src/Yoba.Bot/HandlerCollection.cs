using System.Collections.Generic;

namespace Yoba.Bot
{
    public class HandlerCollection<TMsg> : List<IHandler<TMsg>>
    {

    }
}