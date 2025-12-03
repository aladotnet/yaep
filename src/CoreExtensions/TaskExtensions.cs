using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

/// <summary>
/// Task Extensions
/// </summary>
public static class TaskExtensions
{
    // Cached tasks for common values to avoid allocations
    private static readonly Task<bool> TrueTask = Task.FromResult(true);
    private static readonly Task<bool> FalseTask = Task.FromResult(false);
    private static readonly Task<int> ZeroIntTask = Task.FromResult(0);
    private static readonly Task<int> OneIntTask = Task.FromResult(1);
    private static readonly Task<string> EmptyStringTask = Task.FromResult(string.Empty);
    private static readonly Task<string?> NullStringTask = Task.FromResult<string?>(null);

    /// <summary>
    /// Configures an awaiter used to await this <see cref="Task"/> without an
    /// attempt to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ConfiguredTaskAwaitable DetachedAwait(this Task task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter used to await this <see cref="Task"/> without an
    /// attempt to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ConfiguredTaskAwaitable<T> DetachedAwait<T>(this Task<T> task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter used to await this <see cref="Task"/> without an
    /// attempt to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ConfiguredValueTaskAwaitable DetachedAwait(this ValueTask task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter used to await this <see cref="Task"/> without an
    /// attempt to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ConfiguredValueTaskAwaitable<T> DetachedAwait<T>(this ValueTask<T> task) => task.ConfigureAwait(false);

    /// <summary>
    /// gets a Task&lt;T&gt; from the T. Uses cached tasks for common values (true, false, 0, 1, empty string, null).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static Task<T> AsTaskFromResult<T>(this T value)
    {
        // Return cached tasks for common values to avoid allocation
        if (typeof(T) == typeof(bool))
        {
            var boolValue = (bool)(object)value!;
            return (Task<T>)(object)(boolValue ? TrueTask : FalseTask);
        }

        if (typeof(T) == typeof(int))
        {
            var intValue = (int)(object)value!;
            if (intValue == 0) return (Task<T>)(object)ZeroIntTask;
            if (intValue == 1) return (Task<T>)(object)OneIntTask;
        }

        if (typeof(T) == typeof(string))
        {
            var stringValue = (string?)(object?)value;
            if (stringValue is null) return (Task<T>)(object)NullStringTask!;
            if (stringValue.Length == 0) return (Task<T>)(object)EmptyStringTask;
        }

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
