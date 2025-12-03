using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

/// <summary>
/// Provides extension methods for <see cref="Task"/>, <see cref="Task{TResult}"/>, and <see cref="ValueTask"/> operations.
/// </summary>
/// <remarks>
/// <para>
/// This class provides convenience methods for common async patterns including:
/// </para>
/// <list type="bullet">
/// <item><description>Detaching from synchronization context with <c>ConfigureAwait(false)</c></description></item>
/// <item><description>Converting values to completed tasks (with caching for common values)</description></item>
/// <item><description>Synchronously waiting for task results</description></item>
/// <item><description>Converting between <see cref="Task"/> and <see cref="ValueTask"/></description></item>
/// </list>
/// <para>
/// For allocation optimization, completed tasks for common values (<c>true</c>, <c>false</c>, <c>0</c>, <c>1</c>,
/// empty string, and <c>null</c>) are cached and reused.
/// </para>
/// </remarks>
public static class TaskExtensions
{
    /// <summary>
    /// Cached completed task containing <c>true</c>.
    /// </summary>
    private static readonly Task<bool> TrueTask = Task.FromResult(true);

    /// <summary>
    /// Cached completed task containing <c>false</c>.
    /// </summary>
    private static readonly Task<bool> FalseTask = Task.FromResult(false);

    /// <summary>
    /// Cached completed task containing <c>0</c>.
    /// </summary>
    private static readonly Task<int> ZeroIntTask = Task.FromResult(0);

    /// <summary>
    /// Cached completed task containing <c>1</c>.
    /// </summary>
    private static readonly Task<int> OneIntTask = Task.FromResult(1);

    /// <summary>
    /// Cached completed task containing <see cref="string.Empty"/>.
    /// </summary>
    private static readonly Task<string> EmptyStringTask = Task.FromResult(string.Empty);

    /// <summary>
    /// Cached completed task containing <c>null</c> string.
    /// </summary>
    private static readonly Task<string?> NullStringTask = Task.FromResult<string?>(null);

    /// <summary>
    /// Configures an awaiter that does not attempt to marshal the continuation back to the original synchronization context.
    /// </summary>
    /// <param name="task">The task to configure.</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable"/> configured with <c>ConfigureAwait(false)</c>.</returns>
    /// <remarks>
    /// <para>
    /// This is equivalent to calling <c>task.ConfigureAwait(false)</c> but provides a more semantic name.
    /// </para>
    /// <para>
    /// Use this method in library code or when you don't need to return to the UI thread after an await.
    /// This can improve performance by avoiding unnecessary context switches.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public async Task&lt;string&gt; FetchDataAsync()
    /// {
    ///     var data = await httpClient.GetStringAsync(url).DetachedAwait();
    ///     return ProcessData(data);
    /// }
    /// </code>
    /// </example>
    public static ConfiguredTaskAwaitable DetachedAwait(this Task task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter that does not attempt to marshal the continuation back to the original synchronization context.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The task to configure.</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/> configured with <c>ConfigureAwait(false)</c>.</returns>
    /// <remarks>
    /// This is the generic version for <see cref="Task{TResult}"/>. See <see cref="DetachedAwait(Task)"/> for more details.
    /// </remarks>
    /// <example>
    /// <code>
    /// public async Task&lt;User&gt; GetUserAsync(int id)
    /// {
    ///     return await repository.FindAsync(id).DetachedAwait();
    /// }
    /// </code>
    /// </example>
    public static ConfiguredTaskAwaitable<T> DetachedAwait<T>(this Task<T> task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter that does not attempt to marshal the continuation back to the original synchronization context.
    /// </summary>
    /// <param name="task">The <see cref="ValueTask"/> to configure.</param>
    /// <returns>A <see cref="ConfiguredValueTaskAwaitable"/> configured with <c>ConfigureAwait(false)</c>.</returns>
    /// <remarks>
    /// This is the <see cref="ValueTask"/> version. See <see cref="DetachedAwait(Task)"/> for more details.
    /// </remarks>
    public static ConfiguredValueTaskAwaitable DetachedAwait(this ValueTask task) => task.ConfigureAwait(false);

    /// <summary>
    /// Configures an awaiter that does not attempt to marshal the continuation back to the original synchronization context.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The <see cref="ValueTask{TResult}"/> to configure.</param>
    /// <returns>A <see cref="ConfiguredValueTaskAwaitable{TResult}"/> configured with <c>ConfigureAwait(false)</c>.</returns>
    /// <remarks>
    /// This is the generic <see cref="ValueTask{TResult}"/> version. See <see cref="DetachedAwait(Task)"/> for more details.
    /// </remarks>
    public static ConfiguredValueTaskAwaitable<T> DetachedAwait<T>(this ValueTask<T> task) => task.ConfigureAwait(false);

    /// <summary>
    /// Creates a completed <see cref="Task{TResult}"/> containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in a completed task.</param>
    /// <returns>A completed <see cref="Task{TResult}"/> containing the value.</returns>
    /// <remarks>
    /// <para>
    /// This method uses cached tasks for common values to avoid allocations:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>bool</c>: <c>true</c> and <c>false</c></description></item>
    /// <item><description><c>int</c>: <c>0</c> and <c>1</c></description></item>
    /// <item><description><c>string</c>: empty string and <c>null</c></description></item>
    /// </list>
    /// <para>
    /// For other values, a new task is created via <see cref="Task.FromResult{TResult}"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // These return cached tasks (no allocation)
    /// Task&lt;bool&gt; trueTask = true.AsTaskFromResult();
    /// Task&lt;int&gt; zeroTask = 0.AsTaskFromResult();
    /// Task&lt;string&gt; emptyTask = string.Empty.AsTaskFromResult();
    ///
    /// // This creates a new task
    /// Task&lt;int&gt; fortyTwoTask = 42.AsTaskFromResult();
    /// </code>
    /// </example>
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
    /// Creates a completed <see cref="ValueTask{TResult}"/> containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in a completed <see cref="ValueTask{TResult}"/>.</param>
    /// <returns>A completed <see cref="ValueTask{TResult}"/> containing the value.</returns>
    /// <remarks>
    /// <para>
    /// <see cref="ValueTask{TResult}"/> is more efficient than <see cref="Task{TResult}"/> for completed results
    /// as it can avoid heap allocation entirely when the result is available synchronously.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public ValueTask&lt;int&gt; GetCachedValueAsync(string key)
    /// {
    ///     if (cache.TryGetValue(key, out int value))
    ///         return value.AsValueTaskFromResult(); // No allocation
    ///     return FetchValueAsync(key); // Returns actual async operation
    /// }
    /// </code>
    /// </example>
    public static ValueTask<T> AsValueTaskFromResult<T>(this T value)
    {
        return ValueTask.FromResult(value);
    }

    /// <summary>
    /// Synchronously waits for the task to complete and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The task to wait for.</param>
    /// <returns>The result of the completed task.</returns>
    /// <remarks>
    /// <para>
    /// This method uses <c>GetAwaiter().GetResult()</c> which unwraps exceptions properly
    /// (unlike <see cref="Task{TResult}.Result"/> which wraps them in <see cref="AggregateException"/>).
    /// </para>
    /// <para>
    /// <strong>Warning:</strong> Blocking on async code can cause deadlocks in UI or ASP.NET contexts.
    /// Use this method only when you must block on an async operation and are certain it won't deadlock.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Use sparingly - prefer async/await
    /// var result = SomeAsyncMethod().GetAwaiterResult();
    /// </code>
    /// </example>
    public static T GetAwaiterResult<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Configures the task to not capture the synchronization context.
    /// </summary>
    /// <param name="task">The task to configure.</param>
    /// <returns>The same task instance (for chaining).</returns>
    /// <remarks>
    /// <para>
    /// <strong>Note:</strong> This method calls <c>ConfigureAwait(false)</c> but returns the original <see cref="Task"/>,
    /// not a <see cref="ConfiguredTaskAwaitable"/>. The configuration only affects the task if it's awaited
    /// after this call in the same expression.
    /// </para>
    /// <para>
    /// For most use cases, prefer <see cref="DetachedAwait(Task)"/> which returns the proper <see cref="ConfiguredTaskAwaitable"/>.
    /// </para>
    /// </remarks>
    public static Task WithoutContext(this Task task)
    {
        task.ConfigureAwait(false);
        return task;
    }

    /// <summary>
    /// Configures the task to not capture the synchronization context.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The task to configure.</param>
    /// <returns>The same task instance (for chaining).</returns>
    /// <remarks>
    /// See <see cref="WithoutContext(Task)"/> for more details.
    /// For most use cases, prefer <see cref="DetachedAwait{T}(Task{T})"/>.
    /// </remarks>
    public static Task WithoutContext<T>(this Task<T> task)
    {
        task.ConfigureAwait(false);
        return task;
    }

    /// <summary>
    /// Wraps a <see cref="Task{TResult}"/> in a <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that wraps the task.</returns>
    /// <remarks>
    /// <para>
    /// This is useful when implementing interfaces that return <see cref="ValueTask{TResult}"/>
    /// but your implementation uses <see cref="Task{TResult}"/> internally.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public ValueTask&lt;int&gt; GetValueAsync()
    /// {
    ///     return _httpClient.GetIntAsync().AsValueTask();
    /// }
    /// </code>
    /// </example>
    public static ValueTask<T> AsValueTask<T>(this Task<T> task) => new ValueTask<T>(task);

    /// <summary>
    /// Wraps a <see cref="Task"/> in a <see cref="ValueTask"/>.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <returns>A <see cref="ValueTask"/> that wraps the task.</returns>
    /// <remarks>
    /// This is the non-generic version for tasks that don't return a value.
    /// See <see cref="AsValueTask{T}(Task{T})"/> for more details.
    /// </remarks>
    public static ValueTask AsValueTask(this Task task) => new ValueTask(task);

}
