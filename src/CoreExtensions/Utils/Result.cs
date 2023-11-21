namespace Core.Utils;

public record Result
{
    public string Message { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Exception Exception { get; }

    public Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result From(Result givenResult)
     => givenResult.IsSuccess
        ? Result.Success(givenResult.Message)
        : Result.Failure(givenResult.Exception, givenResult.Message);


    public static Result Success(string message = "")
    {
        return new Result(true, message);
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }
    public static Result Failure<TException>(TException exception, string message)
    where TException : Exception
    {
        return new Result(false, message);
    }

    public static Result<TValue> Failure<TException, TValue>(TException exception, TValue value = default, string message = "")
    where TException : Exception
    {
        return new Result<TValue>(false, value, message);
    }


    public static Result Failure(IEnumerable<string> messages)
        => new Result(false, string.Join(", ", messages));

    public static Result<TValue> Success<TValue>(TValue value, string message = "")
    {
        return new Result<TValue>(true, value, message);
    }

    public static Result<TValue> Failure<TValue>(string message, TValue value)
    {
        return new Result<TValue>(false, value, message);
    }

    public static Result<TValue> Failure<TValue>(string message)
    {
        return new Result<TValue>(false, default, message);
    }

    public static Result<TValue> Failure<TValue>(IEnumerable<string> messages, TValue value)
            => new Result<TValue>(false, value, string.Join(", ", messages));

    public static Result<TValue> Failure<TValue>(IEnumerable<string> messages)
        => new Result<TValue>(false, default, string.Join(", ", messages));
}
