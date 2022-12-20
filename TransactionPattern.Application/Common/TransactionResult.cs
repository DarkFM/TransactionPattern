using System.Collections.Immutable;

namespace TransactionPattern.Application.Common;

public abstract record Result
{
    public abstract bool Succeeded { get; }
    public virtual IImmutableList<ResultError> Errors { get; protected set; } = ImmutableArray<ResultError>.Empty;


    public static Result Success()
    {
        return new SuccessfulResult();
    }


    public static Result Failed(params ResultError[] errors)
    {
        return new FailedResult(errors);
    }


    private record SuccessfulResult : Result
    {
        public override bool Succeeded => true;
    }

    private record FailedResult : Result
    {
        public override bool Succeeded => false;

        public FailedResult(params ResultError[] errors)
        {
            Errors = errors.ToImmutableList();
        }
    }
}


public abstract record Result<T> : Result
{
    public virtual T? Value { get; private set; }

    public static Result<T> Success(T? value)
    {
        return new SuccessfulResult(value);
    }

    public static new Result<T> Failed(params ResultError[] errors)
    {
        return new FailedResult(errors);
    }

    private record SuccessfulResult : Result<T>
    {
        public override bool Succeeded => true;

        public SuccessfulResult(T result = default)
        {
            Value = result;
        }
    }

    private record FailedResult : Result<T>
    {
        public override bool Succeeded => false;

        public FailedResult(params ResultError[] errors)
        {
            Errors = errors.ToImmutableList();
        }
    }
}

public class ResultError
{
    public string Message { get; }
    public Exception? Exception { get; }

    public ResultError(string message, Exception? exception = null)
    {
        Message = message;
        Exception = exception;
    }
}