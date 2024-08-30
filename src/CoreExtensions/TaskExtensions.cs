using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

/// <summary>
/// Task Extensions
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// gets a Task<typeparamref name="T"/> from the T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static Task<T> AsTaskFromResult<T>(this T value)
    {
        return Task.FromResult(value);
    }

    /// <summary>
    /// gets a Task<typeparamref name="T"/> from the T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static ValueTask<T> AsValueTaskFromResult<T>(this T value)
    {
        return ValueTask.FromResult(value);
    }

    /// <summary>
    /// gets the result from a Task calling GetAwaiter().GetResult()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static T GetAwaiterResult<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Configure the given task with ConfigureAwait(false)
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static Task WithoutContext(this Task task)
    {
        task.ConfigureAwait(false);
        return task;
    }

    /// <summary>
    /// Configure the given task with ConfigureAwait(false)
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static Task WithoutContext<T>(this Task<T> task)
    {
        task.ConfigureAwait(false);
        return task;
    }

    /// <summary>
    /// retruns a ValueTask.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ValueTask<T> AsValueTask<T>(this Task<T> task) => new ValueTask<T>(task);

    /// <summary>
    /// retruns a ValueTask.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ValueTask AsValueTask(this Task task)=> new ValueTask(task);

}