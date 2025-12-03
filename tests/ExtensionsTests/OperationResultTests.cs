using FluentAssertions;
using NCommandBus.Core.Abstractions;

namespace ExtensionsTests;

/// <summary>
/// Tests for OperationResult and OperationResult&lt;T&gt; types.
/// </summary>
public class OperationResultTests
{
    #region OperationResult.Success

    /// <summary>
    /// Path: Create success result -> IsSuccess is true, IsFailure is false, Message is set.
    /// </summary>
    [Fact]
    public void Success_CreatesSuccessResult_WithCorrectProperties()
    {
        var message = "Operation completed successfully";

        var result = OperationResult.Success(message);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Message.Should().Be(message);
    }

    #endregion

    #region OperationResult.Failure

    /// <summary>
    /// Path: Create failure result -> IsSuccess is false, IsFailure is true, Message is set.
    /// </summary>
    [Fact]
    public void Failure_CreatesFailureResult_WithCorrectProperties()
    {
        var message = "Operation failed";

        var result = OperationResult.Failure(message);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(message);
    }

    #endregion

    #region OperationResult<T>.Success

    /// <summary>
    /// Path: Create generic success result -> IsSuccess is true, Value is set, Message is set.
    /// </summary>
    [Fact]
    public void SuccessGeneric_CreatesSuccessResult_WithValueAndMessage()
    {
        var message = "Data retrieved";
        var value = 42;

        var result = OperationResult.Success(message, value);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Message.Should().Be(message);
        result.Value.Should().Be(value);
    }

    /// <summary>
    /// Path: Create generic success result with complex type -> Value contains the object.
    /// </summary>
    [Fact]
    public void SuccessGeneric_WithComplexType_ReturnsCorrectValue()
    {
        var message = "User found";
        var user = new TestUser { Id = 1, Name = "John" };

        var result = OperationResult.Success(message, user);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(1);
        result.Value.Name.Should().Be("John");
    }

    #endregion

    #region OperationResult<T>.Failure

    /// <summary>
    /// Path: Create generic failure result -> IsFailure is true, Value is default.
    /// </summary>
    [Fact]
    public void FailureGeneric_CreatesFailureResult_WithDefaultValue()
    {
        var message = "User not found";

        var result = OperationResult.Failure<TestUser>(message);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(message);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Path: Create generic failure result for value type -> Value is default(T).
    /// </summary>
    [Fact]
    public void FailureGeneric_WithValueType_ReturnsDefaultValue()
    {
        var message = "Calculation failed";

        var result = OperationResult.Failure<int>(message);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().Be(default(int));
    }

    #endregion

    #region IsFailure computed property

    /// <summary>
    /// Path: Success result -> IsFailure computed as !IsSuccess -> returns false.
    /// </summary>
    [Fact]
    public void IsFailure_WhenSuccess_ReturnsFalse()
    {
        var result = OperationResult.Success("ok");

        result.IsFailure.Should().BeFalse();
    }

    /// <summary>
    /// Path: Failure result -> IsFailure computed as !IsSuccess -> returns true.
    /// </summary>
    [Fact]
    public void IsFailure_WhenFailure_ReturnsTrue()
    {
        var result = OperationResult.Failure("error");

        result.IsFailure.Should().BeTrue();
    }

    #endregion

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
