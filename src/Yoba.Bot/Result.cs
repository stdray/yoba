using System;

namespace Yoba.Bot
{
    public class Result
    {
        internal Result(Status status, Exception error)
        {
            Status = status;
            Exception = error;
        }

        public static Result Success() => new Result(Status.Success, null);
        public static Result Error(Exception error) => new Result(Status.Fail, error);

        public static Result Skip () => new Result(Status.None, null);

        public Exception Exception { get; }

        public Status Status { get; }
    }

    public class Result<TRsp> : Result
    {
        public static Result<TRsp> Success(TRsp response) => 
            new Result<TRsp>(Status.Success, response);

        Result(Status status, TRsp response) : base(status, null)
        {
            Response = response;
        }

        public TRsp Response { get; }
    }
}