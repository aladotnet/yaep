using FluentAssertions;

namespace ExtensionsTests;

/// <summary>
/// Tests for EnumExtensions methods.
/// </summary>
public class EnumExtensionsTests
{
    private enum TestEnum
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3
    }

    #region ToEnum with default value

    /// <summary>
    /// Path: Valid string value -> successful parse -> returns parsed enum value.
    /// </summary>
    [Fact]
    public void ToEnum_WithDefault_GivenValidString_ReturnsParsedValue()
    {
        var result = "First".ToEnum(TestEnum.None);

        result.Should().Be(TestEnum.First);
    }

    /// <summary>
    /// Path: Valid string value (case insensitive) -> successful parse -> returns parsed enum value.
    /// </summary>
    [Fact]
    public void ToEnum_WithDefault_GivenValidStringDifferentCase_ReturnsParsedValue()
    {
        var result = "SECOND".ToEnum(TestEnum.None);

        result.Should().Be(TestEnum.Second);
    }

    /// <summary>
    /// Path: Invalid string value -> parse fails -> returns default value.
    /// </summary>
    [Fact]
    public void ToEnum_WithDefault_GivenInvalidString_ReturnsDefaultValue()
    {
        var result = "InvalidValue".ToEnum(TestEnum.Third);

        result.Should().Be(TestEnum.Third);
    }

    /// <summary>
    /// Path: Null string value -> parse fails -> returns default value.
    /// </summary>
    [Fact]
    public void ToEnum_WithDefault_GivenNull_ReturnsDefaultValue()
    {
        string? value = null;

        var result = value.ToEnum(TestEnum.Second);

        result.Should().Be(TestEnum.Second);
    }

    /// <summary>
    /// Path: Empty string value -> parse fails -> returns default value.
    /// </summary>
    [Fact]
    public void ToEnum_WithDefault_GivenEmptyString_ReturnsDefaultValue()
    {
        var result = "".ToEnum(TestEnum.First);

        result.Should().Be(TestEnum.First);
    }

    #endregion

    #region GetValues

    /// <summary>
    /// Path: Valid enum value -> type is enum -> returns all enum values.
    /// </summary>
    [Fact]
    public void GetValues_GivenEnumValue_ReturnsAllValues()
    {
        var result = TestEnum.None.GetValues();

        result.Should().BeEquivalentTo(new[] { TestEnum.None, TestEnum.First, TestEnum.Second, TestEnum.Third });
    }

    /// <summary>
    /// Path: Non-enum struct value -> guard fails -> throws ArgumentException.
    /// </summary>
    [Fact]
    public void GetValues_GivenNonEnumStruct_ThrowsArgumentException()
    {
        var notAnEnum = 42;

        var act = () => notAnEnum.GetValues();

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GetNames

    /// <summary>
    /// Path: Valid enum value -> type is enum -> returns all enum names.
    /// </summary>
    [Fact]
    public void GetNames_GivenEnumValue_ReturnsAllNames()
    {
        var result = TestEnum.First.GetNames();

        result.Should().BeEquivalentTo(new[] { "None", "First", "Second", "Third" });
    }

    /// <summary>
    /// Path: Non-enum struct value -> guard fails -> throws ArgumentException.
    /// </summary>
    [Fact]
    public void GetNames_GivenNonEnumStruct_ThrowsArgumentException()
    {
        var notAnEnum = 42;

        var act = () => notAnEnum.GetNames();

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ToNameValuePares

    /// <summary>
    /// Path: Valid enum value -> gets names and values -> returns tuples of (name, value).
    /// </summary>
    [Fact]
    public void ToNameValuePares_GivenEnumValue_ReturnsNameValueTuples()
    {
        var result = TestEnum.None.ToNameValuePares();

        result.Should().HaveCount(4);
        result.Should().Contain(("None", 0));
        result.Should().Contain(("First", 1));
        result.Should().Contain(("Second", 2));
        result.Should().Contain(("Third", 3));
    }

    #endregion

    #region GetValue

    /// <summary>
    /// Path: Valid enum and valid name -> finds matching name -> returns corresponding int value.
    /// </summary>
    [Fact]
    public void GetValue_GivenValidName_ReturnsIntValue()
    {
        var result = TestEnum.None.GetValue("Second");

        result.Should().Be(2);
    }

    /// <summary>
    /// Path: Valid enum and valid name (case insensitive) -> finds matching name -> returns value.
    /// </summary>
    [Fact]
    public void GetValue_GivenValidNameDifferentCase_ReturnsIntValue()
    {
        var result = TestEnum.None.GetValue("THIRD");

        result.Should().Be(3);
    }

    /// <summary>
    /// Path: Valid enum and invalid name -> no match found -> throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void GetValue_GivenInvalidName_ThrowsInvalidOperationException()
    {
        var act = () => TestEnum.None.GetValue("NotExists");

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Path: Valid enum and null/empty name -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetValue_GivenNullName_ThrowsArgumentNullException()
    {
        var act = () => TestEnum.None.GetValue(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Path: Non-enum struct -> guard fails -> throws ArgumentException.
    /// </summary>
    [Fact]
    public void GetValue_GivenNonEnumStruct_ThrowsArgumentException()
    {
        var notAnEnum = 42;

        var act = () => notAnEnum.GetValue("Any");

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GetName

    /// <summary>
    /// Path: Valid enum and valid int value -> finds matching value -> returns corresponding name.
    /// </summary>
    [Fact]
    public void GetName_GivenValidIntValue_ReturnsName()
    {
        var result = TestEnum.None.GetName(2);

        result.Should().Be("Second");
    }

    /// <summary>
    /// Path: Valid enum and invalid int value -> no match found -> throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void GetName_GivenInvalidIntValue_ThrowsInvalidOperationException()
    {
        var act = () => TestEnum.None.GetName(999);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Path: Non-enum struct -> guard fails -> throws ArgumentException.
    /// </summary>
    [Fact]
    public void GetName_GivenNonEnumStruct_ThrowsArgumentException()
    {
        var notAnEnum = 42;

        var act = () => notAnEnum.GetName(1);

        act.Should().Throw<ArgumentException>();
    }

    #endregion
}
