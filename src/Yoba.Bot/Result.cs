using System;

namespace Yoba.Bot
{
    public class Result
    {
        Result(HandleStatus status, Exception error)
        {
            Status = status;
            Exception = error;
        }

        public static Result Success = new Result(HandleStatus.Success, null);
        public static Result Error(Exception error) => new Result(HandleStatus.Fail, error);
        
        public static Result Skip = new Result(HandleStatus.Fail, null);

        public Exception Exception { get; }

        public HandleStatus Status { get; }
    }

//    public class Result<TMsg> : Result
//    {
//        public Result(HandleStatus status, Request<TMsg> request) : base(status)
//        {
//            Request = request;
//            Status = status;
//        }
//
//        public Request<TMsg> Request { get; }
//        public HandleStatus Status { get; }
//    }
}