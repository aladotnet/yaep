namespace NCommandBus.Core.Abstractions;

/// <summary>
/// Represents an Result that can be returned from an operation call.
/// </summary>
public record OperationResult
{
    /// <summary>
    /// Gets the result message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets wheather the operation succeeded or not.
    /// </summary>
    public bool  IsSuccess { get; }

    /// <summary>
    /// Gets wheather the operation is a failure or not.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates an instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="success"></param>
    protected OperationResult(string message, bool success)
    {
        Message = message;
        IsSuccess = success;
    }

    /// <summary>
    /// Creates an failure Result
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static OperationResult Failure(string message) => new(message, false);

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static OperationResult Success(string message) => new(message, true);

    /// <summary>
    /// Creates an failure Result
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static OperationResult<T?> Failure<T>(string message) => new(default,message, false);

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static OperationResult<T> Success<T>(string message,T value) => new(value,message, true);
}

/// <summary>
/// Represents a Result vlaue
/// </summary>
/// <typeparam name="T"></typeparam>
public record OperationResult<T> : OperationResult
{
    /// <summary>
    /// Gets the result content. 
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <param name="success"></param>
    internal OperationResult(T value,string message, bool success) : base(message, success)
    {
        Value = value;
    }
}
