using System;

namespace Yoba.Bot
{
    [Flags]
    public enum Status
    {
        None = 0b_0000_0000,
        Fail = 0b_0000_0001,
        Success = 0b_0000_0010,
    }
}