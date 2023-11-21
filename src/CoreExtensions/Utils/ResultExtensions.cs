using System;
using System.Threading.Tasks;

namespace Core.Utils;

public static class ResultExtensions
{
    public static Result OnFailure(this Result result, Action<Result> action)
    {
        if (result.IsFailure)
            action(result);

        return result;
    }

    public static Result From(this Result result, Result givenResult)
        => givenResult.IsSuccess 
           ? Result.Success(result.Message) 
           : Result.Failure(givenResult.Exception, givenResult.Message);


    public static Task<Result> OnSuccess(this Result result, Func<Task<Result>> func)
    {
        if (result.IsSuccess)
            return func();

        return result.AsTaskFromResult();
    }

    public static Task<Result<T>> OnSuccess<T>(this Result<T> result, Func<Result<T>,Task<Result<T>>> func)
    {
        if (result.IsSuccess)
            return func(result);

        return result.AsTaskFromResult();
    }

    public static Task<Result<T>> OnFailure<T>(this Result<T> result, Func<Result<T>, Task<Result<T>>> func)
    {
        if (result.IsFailure)
            return func(result);

        return result.AsTaskFromResult();
    }

    public static Task<Result> OnSuccess(this Result result, Func<Result,Task<Result>> func)
    {
        if (result.IsSuccess)
            return func(result);

        return result.AsTaskFromResult();
    }


    public static async Task<Result> OnSuccess(this Task<Result> task, Func<Task<Result>> func)
    {
        var result = await task;

        if (result.IsSuccess)
            return await func();

        return result;
    }

    public static async Task<Result> OnFailure(this Task<Result> task, Func<Task<Result>> func)
    {
        var result = await task;

        if (result.IsFailure)
            return await func();

        return result;
    }

    public static async Task<Result> OnFailure(this Task<Result> task, Func<Result, Task<Result>> func)
    {
        var result = await task;

        if (result.IsFailure)
            return await func(result);

        return result;
    }
}
