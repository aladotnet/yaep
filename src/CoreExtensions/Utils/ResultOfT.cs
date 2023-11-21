namespace Core.Utils;

public record Result<T> : Result
{
    public T Value { get; }
    public Result(bool isSuccess, T value, string message) : base(isSuccess, message)
    {
        Value = value;
    }
}
