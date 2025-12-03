using FluentAssertions;
using System.Text;

namespace ExtensionsTests;

/// <summary>
/// Tests for StringExtensions methods.
/// </summary>
public class StringExtensionsTests
{
    #region IsNullOrEmpty

    /// <summary>
    /// Path: Null string -> string.IsNullOrWhiteSpace returns true -> returns true.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_GivenNull_ReturnsTrue()
    {
        string? value = null;

        value.IsNullOrEmpty().Should().BeTrue();
    }

    /// <summary>
    /// Path: Empty string -> string.IsNullOrWhiteSpace returns true -> returns true.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_GivenEmptyString_ReturnsTrue()
    {
        var value = "";

        value.IsNullOrEmpty().Should().BeTrue();
    }

    /// <summary>
    /// Path: Whitespace string -> string.IsNullOrWhiteSpace returns true -> returns true.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_GivenWhitespace_ReturnsTrue()
    {
        var value = "   ";

        value.IsNullOrEmpty().Should().BeTrue();
    }

    /// <summary>
    /// Path: Non-empty string -> string.IsNullOrWhiteSpace returns false -> returns false.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_GivenNonEmptyString_ReturnsFalse()
    {
        var value = "Test";

        value.IsNullOrEmpty().Should().BeFalse();
    }

    #endregion

    #region EqualsIgnoreCaseInvariant

    /// <summary>
    /// Path: Same strings different case -> InvariantCultureIgnoreCase comparison -> returns true.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseInvariant_GivenSameStringDifferentCase_ReturnsTrue()
    {
        var result = "Hello".EqualsIgnoreCaseInvariant("HELLO");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Different strings -> InvariantCultureIgnoreCase comparison -> returns false.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseInvariant_GivenDifferentStrings_ReturnsFalse()
    {
        var result = "Hello".EqualsIgnoreCaseInvariant("World");

        result.Should().BeFalse();
    }

    #endregion

    #region EqualsIgnoreCaseOrdinal

    /// <summary>
    /// Path: Same strings different case -> OrdinalIgnoreCase comparison -> returns true.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseOrdinal_GivenSameStringDifferentCase_ReturnsTrue()
    {
        var result = "Test".EqualsIgnoreCaseOrdinal("TEST");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Different strings -> OrdinalIgnoreCase comparison -> returns false.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseOrdinal_GivenDifferentStrings_ReturnsFalse()
    {
        var result = "Test".EqualsIgnoreCaseOrdinal("Other");

        result.Should().BeFalse();
    }

    #endregion

    #region EqualsIgnoreCaseCurrent

    /// <summary>
    /// Path: Same strings different case -> CurrentCultureIgnoreCase comparison -> returns true.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseCurrent_GivenSameStringDifferentCase_ReturnsTrue()
    {
        var result = "Value".EqualsIgnoreCaseCurrent("VALUE");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Different strings -> CurrentCultureIgnoreCase comparison -> returns false.
    /// </summary>
    [Fact]
    public void EqualsIgnoreCaseCurrent_GivenDifferentStrings_ReturnsFalse()
    {
        var result = "Value".EqualsIgnoreCaseCurrent("Different");

        result.Should().BeFalse();
    }

    #endregion

    #region EmptyIfNull

    /// <summary>
    /// Path: Null string -> null coalescing -> returns empty string.
    /// </summary>
    [Fact]
    public void EmptyIfNull_GivenNull_ReturnsEmptyString()
    {
        string? value = null;

        var result = value.EmptyIfNull();

        result.Should().BeEmpty();
    }

    /// <summary>
    /// Path: Non-null string -> null coalescing -> returns original value.
    /// </summary>
    [Fact]
    public void EmptyIfNull_GivenNonNull_ReturnsOriginalValue()
    {
        var value = "test";

        var result = value.EmptyIfNull();

        result.Should().Be("test");
    }

    #endregion

    #region ToByteArray

    /// <summary>
    /// Path: Valid string, default encoding -> UTF8 encoding -> returns byte array.
    /// </summary>
    [Fact]
    public void ToByteArray_GivenValidString_ReturnsUtf8Bytes()
    {
        var value = "Hello";

        var result = value.ToByteArray();

        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Hello"));
    }

    /// <summary>
    /// Path: Valid string, custom encoding -> uses provided encoding -> returns byte array.
    /// </summary>
    [Fact]
    public void ToByteArray_GivenCustomEncoding_UsesProvidedEncoding()
    {
        var value = "Test";

        var result = value.ToByteArray(Encoding.ASCII);

        result.Should().BeEquivalentTo(Encoding.ASCII.GetBytes("Test"));
    }

    /// <summary>
    /// Path: Null/empty string -> IsNullOrEmpty is true -> returns empty array.
    /// </summary>
    [Fact]
    public void ToByteArray_GivenNullOrEmpty_ReturnsEmptyArray()
    {
        var value = "";

        var result = value.ToByteArray();

        result.Should().BeEmpty();
    }

    #endregion

    #region Concat

    /// <summary>
    /// Path: Valid string and values -> concatenates all -> returns combined string.
    /// </summary>
    [Fact]
    public void Concat_GivenValidStringAndValues_ReturnsConcatenatedString()
    {
        var result = "Hello".Concat(new[] { " ", "World", "!" });

        result.Should().Be("Hello World!");
    }

    /// <summary>
    /// Path: Valid string and null/empty values -> values check returns early -> returns original.
    /// </summary>
    [Fact]
    public void Concat_GivenEmptyValues_ReturnsOriginalString()
    {
        var result = "Hello".Concat(Array.Empty<string>());

        result.Should().Be("Hello");
    }

    /// <summary>
    /// Path: Null/empty source string -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void Concat_GivenNullSource_ThrowsArgumentNullException()
    {
        string? value = null;

        var act = () => value!.Concat(new[] { "test" });

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region IsGuidValue

    /// <summary>
    /// Path: Valid GUID string -> Guid.TryParse succeeds -> returns true.
    /// </summary>
    [Fact]
    public void IsGuidValue_GivenValidGuidString_ReturnsTrue()
    {
        var value = "12345678-1234-1234-1234-123456789012";

        var result = value.IsGuidValue();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Invalid GUID string -> Guid.TryParse fails -> returns false.
    /// </summary>
    [Fact]
    public void IsGuidValue_GivenInvalidGuidString_ReturnsFalse()
    {
        var value = "not-a-guid";

        var result = value.IsGuidValue();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Empty string -> Guid.TryParse fails -> returns false.
    /// </summary>
    [Fact]
    public void IsGuidValue_GivenEmptyString_ReturnsFalse()
    {
        var value = "";

        var result = value.IsGuidValue();

        result.Should().BeFalse();
    }

    #endregion

    #region IsNumeric

    /// <summary>
    /// Path: Numeric string -> all chars are digits -> returns true.
    /// </summary>
    [Fact]
    public void IsNumeric_GivenNumericString_ReturnsTrue()
    {
        var value = "12345";

        var result = value.IsNumeric();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Non-numeric string -> not all chars are digits -> returns false.
    /// </summary>
    [Fact]
    public void IsNumeric_GivenNonNumericString_ReturnsFalse()
    {
        var value = "123abc";

        var result = value.IsNumeric();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Empty string -> string.IsNullOrEmpty returns true -> returns false.
    /// </summary>
    [Fact]
    public void IsNumeric_GivenEmptyString_ReturnsFalse()
    {
        var value = "";

        var result = value.IsNumeric();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: String with decimal point -> not all chars are digits -> returns false.
    /// </summary>
    [Fact]
    public void IsNumeric_GivenDecimalString_ReturnsFalse()
    {
        var value = "123.45";

        var result = value.IsNumeric();

        result.Should().BeFalse();
    }

    #endregion

    #region DefaultIfNull

    /// <summary>
    /// Path: Null string -> IsNullOrEmpty returns true -> returns default value.
    /// </summary>
    [Fact]
    public void DefaultIfNull_GivenNull_ReturnsDefaultValue()
    {
        string? value = null;
        var defaultValue = "default";

        var result = value.DefaultIfNull(defaultValue);

        result.Should().Be(defaultValue);
    }

    /// <summary>
    /// Path: Non-null string -> IsNullOrEmpty returns false -> returns original value.
    /// </summary>
    [Fact]
    public void DefaultIfNull_GivenNonNull_ReturnsOriginalValue()
    {
        var value = "test";
        var defaultValue = "default";

        var result = value.DefaultIfNull(defaultValue);

        result.Should().Be(value);
    }

    /// <summary>
    /// Path: Empty string -> IsNullOrEmpty returns true -> returns default value.
    /// </summary>
    [Fact]
    public void DefaultIfNull_GivenEmptyString_ReturnsDefaultValue()
    {
        var value = "";
        var defaultValue = "default";

        var result = value.DefaultIfNull(defaultValue);

        result.Should().Be(defaultValue);
    }

    #endregion
}
