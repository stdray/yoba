using System;

namespace Yoba.Bot
{
    [Flags]
    public enum HandleStatus
    {
        None = 0b_0000_0000,
        Fail = 0b_0000_0001,
        Success = 0b_0000_0010,
        Stop = 0b_0000_0100,
    }
}