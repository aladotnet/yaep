using FluentAssertions;

namespace ExtensionsTests;

/// <summary>
/// Tests for ExceptionExtensions methods.
/// </summary>
public class ExceptionExtensionsTests
{
    #region GuardAgainstNull with parameter name

    /// <summary>
    /// Path: Value is null -> throws ArgumentNullException with parameter name.
    /// </summary>
    [Fact]
    public void GuardAgainstNull_GivenNull_ThrowsArgumentNullException()
    {
        Person? person = null;

        var act = () => person.GuardAgainstNull(nameof(person));

        act.Should().Throw<ArgumentNullException>()
           .And.ParamName.Should().Be("person");
    }

    /// <summary>
    /// Path: Value is not null -> returns the value unchanged.
    /// </summary>
    [Fact]
    public void GuardAgainstNull_GivenNonNull_ReturnsValue()
    {
        var person = new Person("Test", "Test");

        var result = person.GuardAgainstNull(nameof(person));

        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("Test");
    }

    #endregion

    #region GuardAgainstNull with custom exception

    /// <summary>
    /// Path: Value is null -> throws the provided custom exception.
    /// </summary>
    [Fact]
    public void GuardAgainstNull_WithCustomException_GivenNull_ThrowsCustomException()
    {
        string? value = null;
        var customException = new InvalidOperationException("Custom error");

        var act = () => value.GuardAgainstNull(customException);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Custom error");
    }

    /// <summary>
    /// Path: Value is not null -> returns the value, does not throw.
    /// </summary>
    [Fact]
    public void GuardAgainstNull_WithCustomException_GivenNonNull_ReturnsValue()
    {
        var value = "test";
        var customException = new InvalidOperationException("Custom error");

        var result = value.GuardAgainstNull(customException);

        result.Should().Be("test");
    }

    #endregion

    #region GuardAgainst with predicate and message

    /// <summary>
    /// Path: Predicate returns true -> throws ArgumentException with message.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithMessage_WhenPredicateTrue_ThrowsArgumentException()
    {
        var value = 42;

        var act = () => value.GuardAgainst(v => v == 42, "Value cannot be 42");

        act.Should().Throw<ArgumentException>()
           .WithMessage("Value cannot be 42");
    }

    /// <summary>
    /// Path: Predicate returns false -> returns the value unchanged.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithMessage_WhenPredicateFalse_ReturnsValue()
    {
        var value = 42;

        var result = value.GuardAgainst(v => v == -1, "Value cannot be -1");

        result.Should().Be(42);
    }

    /// <summary>
    /// Path: Null predicate -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithMessage_GivenNullPredicate_ThrowsArgumentNullException()
    {
        var value = 42;

        var act = () => value.GuardAgainst(null!, "message");

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region GuardAgainst with predicate and exception

    /// <summary>
    /// Path: Predicate returns true -> throws the provided exception.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithException_WhenPredicateTrue_ThrowsProvidedException()
    {
        var value = 42;

        var act = () => value.GuardAgainst(v => v == 42, new ArgumentException("Value is invalid"));

        act.Should().Throw<ArgumentException>()
           .WithMessage("Value is invalid");
    }

    /// <summary>
    /// Path: Predicate returns false -> returns the value unchanged.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithException_WhenPredicateFalse_ReturnsValue()
    {
        var value = 42;

        var result = value.GuardAgainst(v => v == -1, new ArgumentException());

        result.Should().Be(42);
    }

    /// <summary>
    /// Path: Custom exception type -> predicate true -> throws custom exception.
    /// </summary>
    [Fact]
    public void GuardAgainst_WithCustomExceptionType_WhenPredicateTrue_ThrowsCustomException()
    {
        var value = "test";

        var act = () => value.GuardAgainst(v => v.Length < 10, new InvalidOperationException("Too short"));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Too short");
    }

    #endregion

    #region GuardAgainst<T, TException> with predicate and message

    /// <summary>
    /// Path: Predicate returns true -> creates and throws specified exception type with message.
    /// </summary>
    [Fact]
    public void GuardAgainst_GenericException_WhenPredicateTrue_ThrowsSpecifiedExceptionType()
    {
        var value = 100;

        var act = () => value.GuardAgainst<int, InvalidOperationException>(v => v > 50, "Value too large");

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Value too large");
    }

    /// <summary>
    /// Path: Predicate returns false -> returns the value unchanged.
    /// </summary>
    [Fact]
    public void GuardAgainst_GenericException_WhenPredicateFalse_ReturnsValue()
    {
        var value = 25;

        var result = value.GuardAgainst<int, InvalidOperationException>(v => v > 50, "Value too large");

        result.Should().Be(25);
    }

    #endregion

    #region GuardAgainstNullOrEmpty

    /// <summary>
    /// Path: Null string -> throws ArgumentNullException with parameter name.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void GuardAgainstNullOrEmpty_GivenNullOrEmptyOrWhitespace_ThrowsArgumentNullException(string? value)
    {
        var act = () => value.GuardAgainstNullOrEmpty(nameof(value));

        act.Should().Throw<ArgumentNullException>()
           .And.ParamName.Should().Be("value");
    }

    /// <summary>
    /// Path: Non-empty string -> returns the value unchanged.
    /// </summary>
    [Fact]
    public void GuardAgainstNullOrEmpty_GivenNonEmpty_ReturnsValue()
    {
        var value = "Test";

        var result = value.GuardAgainstNullOrEmpty(nameof(value));

        result.Should().Be("Test");
    }

    /// <summary>
    /// Path: Null string with custom message -> throws ArgumentNullException with message.
    /// </summary>
    [Fact]
    public void GuardAgainstNullOrEmpty_WithMessage_GivenNull_ThrowsWithMessage()
    {
        string? value = null;

        var act = () => value.GuardAgainstNullOrEmpty(nameof(value), "Custom error message");

        act.Should().Throw<ArgumentNullException>()
           .And.Message.Should().Contain("Custom error message");
    }

    /// <summary>
    /// Path: Non-empty string with custom message -> returns value (message not used).
    /// </summary>
    [Fact]
    public void GuardAgainstNullOrEmpty_WithMessage_GivenNonEmpty_ReturnsValue()
    {
        var value = "Valid";

        var result = value.GuardAgainstNullOrEmpty(nameof(value), "Not used");

        result.Should().Be("Valid");
    }

    #endregion
}
