using FluentAssertions;

namespace ExtensionsTests;

/// <summary>
/// Tests for TaskExtensions methods.
/// </summary>
public class TaskExtensionsTests
{
    #region DetachedAwait (Task)

    /// <summary>
    /// Path: Task -> ConfigureAwait(false) -> returns ConfiguredTaskAwaitable.
    /// </summary>
    [Fact]
    public async Task DetachedAwait_Task_ReturnsConfiguredAwaitable()
    {
        var task = Task.FromResult(42);

        // The DetachedAwait should not throw and should be awaitable
        await task.DetachedAwait();

        // If we get here, the await completed successfully
        task.IsCompletedSuccessfully.Should().BeTrue();
    }

    #endregion

    #region DetachedAwait (Task<T>)

    /// <summary>
    /// Path: Task&lt;T&gt; -> ConfigureAwait(false) -> returns ConfiguredTaskAwaitable&lt;T&gt; with result.
    /// </summary>
    [Fact]
    public async Task DetachedAwait_TaskOfT_ReturnsConfiguredAwaitableWithResult()
    {
        var task = Task.FromResult(42);

        var result = await task.DetachedAwait();

        result.Should().Be(42);
    }

    #endregion

    #region DetachedAwait (ValueTask)

    /// <summary>
    /// Path: ValueTask -> ConfigureAwait(false) -> returns ConfiguredValueTaskAwaitable.
    /// </summary>
    [Fact]
    public async Task DetachedAwait_ValueTask_ReturnsConfiguredAwaitable()
    {
        var valueTask = new ValueTask();

        await valueTask.DetachedAwait();

        // If we get here, the await completed successfully
        valueTask.IsCompletedSuccessfully.Should().BeTrue();
    }

    #endregion

    #region DetachedAwait (ValueTask<T>)

    /// <summary>
    /// Path: ValueTask&lt;T&gt; -> ConfigureAwait(false) -> returns ConfiguredValueTaskAwaitable&lt;T&gt; with result.
    /// </summary>
    [Fact]
    public async Task DetachedAwait_ValueTaskOfT_ReturnsConfiguredAwaitableWithResult()
    {
        var valueTask = new ValueTask<string>("hello");

        var result = await valueTask.DetachedAwait();

        result.Should().Be("hello");
    }

    #endregion

    #region AsTaskFromResult

    /// <summary>
    /// Path: Value -> Task.FromResult -> returns completed Task&lt;T&gt; with value.
    /// </summary>
    [Fact]
    public async Task AsTaskFromResult_GivenValue_ReturnsCompletedTaskWithValue()
    {
        var value = 42;

        var task = value.AsTaskFromResult();

        task.IsCompletedSuccessfully.Should().BeTrue();
        (await task).Should().Be(42);
    }

    /// <summary>
    /// Path: Null value -> Task.FromResult -> returns completed Task&lt;T&gt; with null.
    /// </summary>
    [Fact]
    public async Task AsTaskFromResult_GivenNull_ReturnsCompletedTaskWithNull()
    {
        string? value = null;

        var task = value.AsTaskFromResult();

        task.IsCompletedSuccessfully.Should().BeTrue();
        (await task).Should().BeNull();
    }

    /// <summary>
    /// Path: Complex object -> Task.FromResult -> returns completed Task&lt;T&gt; with object.
    /// </summary>
    [Fact]
    public async Task AsTaskFromResult_GivenComplexObject_ReturnsCompletedTaskWithObject()
    {
        var obj = new TestData { Id = 1, Name = "Test" };

        var task = obj.AsTaskFromResult();

        var result = await task;
        result.Should().BeSameAs(obj);
    }

    #endregion

    #region AsValueTaskFromResult

    /// <summary>
    /// Path: Value -> ValueTask.FromResult -> returns completed ValueTask&lt;T&gt; with value.
    /// </summary>
    [Fact]
    public async Task AsValueTaskFromResult_GivenValue_ReturnsCompletedValueTaskWithValue()
    {
        var value = "test";

        var valueTask = value.AsValueTaskFromResult();

        valueTask.IsCompletedSuccessfully.Should().BeTrue();
        (await valueTask).Should().Be("test");
    }

    #endregion

    #region GetAwaiterResult

    /// <summary>
    /// Path: Completed task -> GetAwaiter().GetResult() -> returns result synchronously.
    /// </summary>
    [Fact]
    public void GetAwaiterResult_GivenCompletedTask_ReturnsResultSynchronously()
    {
        var task = Task.FromResult(123);

        var result = task.GetAwaiterResult();

        result.Should().Be(123);
    }

    /// <summary>
    /// Path: Faulted task -> GetAwaiter().GetResult() -> throws exception.
    /// </summary>
    [Fact]
    public void GetAwaiterResult_GivenFaultedTask_ThrowsException()
    {
        var task = Task.FromException<int>(new InvalidOperationException("Test error"));

        var act = () => task.GetAwaiterResult();

        act.Should().Throw<InvalidOperationException>().WithMessage("Test error");
    }

    #endregion

    #region WithoutContext (Task)

    /// <summary>
    /// Path: Task -> ConfigureAwait(false) called -> returns same task.
    /// </summary>
    [Fact]
    public async Task WithoutContext_Task_ReturnsSameTask()
    {
        var originalTask = Task.FromResult(42);

        var result = originalTask.WithoutContext();

        result.Should().BeSameAs(originalTask);
        await result; // Should complete without issues
    }

    #endregion

    #region WithoutContext (Task<T>)

    /// <summary>
    /// Path: Task&lt;T&gt; -> ConfigureAwait(false) called -> returns same task.
    /// </summary>
    [Fact]
    public async Task WithoutContext_TaskOfT_ReturnsSameTask()
    {
        var originalTask = Task.FromResult("value");

        var result = originalTask.WithoutContext();

        result.Should().BeSameAs(originalTask);
        await result;
    }

    #endregion

    #region AsValueTask (Task<T>)

    /// <summary>
    /// Path: Task&lt;T&gt; -> new ValueTask&lt;T&gt;(task) -> returns ValueTask wrapping the task.
    /// </summary>
    [Fact]
    public async Task AsValueTask_TaskOfT_ReturnsValueTaskWithResult()
    {
        var task = Task.FromResult(42);

        var valueTask = task.AsValueTask();

        var result = await valueTask;
        result.Should().Be(42);
    }

    #endregion

    #region AsValueTask (Task)

    /// <summary>
    /// Path: Task -> new ValueTask(task) -> returns ValueTask wrapping the task.
    /// </summary>
    [Fact]
    public async Task AsValueTask_Task_ReturnsValueTask()
    {
        var task = Task.CompletedTask;

        var valueTask = task.AsValueTask();

        await valueTask;
        valueTask.IsCompletedSuccessfully.Should().BeTrue();
    }

    #endregion

    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
