using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System;

/// <summary>
/// Provides extension methods for <see cref="string"/> operations including null/empty checks,
/// case-insensitive comparisons, encoding conversions, and string manipulation utilities.
/// </summary>
/// <remarks>
/// All methods in this class are optimized for minimal allocations where possible.
/// Methods use <see cref="Span{T}"/> and avoid LINQ to reduce heap allocations.
/// </remarks>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="value"/> is null, empty, or consists only of white-space characters;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="string.IsNullOrWhiteSpace"/> internally.
    /// The <see cref="NotNullWhenAttribute"/> ensures the compiler knows the value is not null when returning <c>false</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// string? name = GetName();
    /// if (!name.IsNullOrEmpty())
    /// {
    ///     // name is guaranteed non-null here
    ///     Console.WriteLine(name.Length);
    /// }
    /// </code>
    /// </example>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Compares two strings for equality using <see cref="StringComparison.InvariantCultureIgnoreCase"/>.
    /// </summary>
    /// <param name="src">The source string to compare.</param>
    /// <param name="value">The string to compare against.</param>
    /// <returns><c>true</c> if the strings are equal (case-insensitive, invariant culture); otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="src"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// bool result = "Hello".EqualsIgnoreCaseInvariant("HELLO"); // true
    /// </code>
    /// </example>
    public static bool EqualsIgnoreCaseInvariant(this string src, string value)
        => src.GuardAgainstNullOrEmpty(nameof(src))
              .Equals(value, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Compares two strings for equality using <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </summary>
    /// <param name="src">The source string to compare.</param>
    /// <param name="value">The string to compare against.</param>
    /// <returns><c>true</c> if the strings are equal (case-insensitive, ordinal); otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="src"/> is null or empty.</exception>
    /// <remarks>
    /// Ordinal comparison is faster than culture-sensitive comparisons and is recommended
    /// for comparing programmatic identifiers, file paths, and other non-linguistic strings.
    /// </remarks>
    /// <example>
    /// <code>
    /// bool result = "Config".EqualsIgnoreCaseOrdinal("CONFIG"); // true
    /// </code>
    /// </example>
    public static bool EqualsIgnoreCaseOrdinal(this string src, string value)
        => src.GuardAgainstNullOrEmpty(nameof(src))
              .Equals(value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Compares two strings for equality using <see cref="StringComparison.CurrentCultureIgnoreCase"/>.
    /// </summary>
    /// <param name="src">The source string to compare.</param>
    /// <param name="value">The string to compare against.</param>
    /// <returns><c>true</c> if the strings are equal (case-insensitive, current culture); otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="src"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// bool result = "Straße".EqualsIgnoreCaseCurrent("STRASSE"); // depends on culture
    /// </code>
    /// </example>
    public static bool EqualsIgnoreCaseCurrent(this string src, string value)
        => src.GuardAgainstNullOrEmpty(nameof(src))
              .Equals(value, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// Returns an empty string if the specified value is null; otherwise, returns the original value.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <returns>An empty string if <paramref name="value"/> is null; otherwise, the original value.</returns>
    /// <remarks>This method is allocation-free as it uses <see cref="string.Empty"/>.</remarks>
    /// <example>
    /// <code>
    /// string? input = null;
    /// string result = input.EmptyIfNull(); // ""
    /// </code>
    /// </example>
    public static string EmptyIfNull(this string? value)
        => value ?? string.Empty;

    /// <summary>
    /// Converts the string to a byte array using the specified encoding.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="encoding">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if not specified.</param>
    /// <returns>A byte array containing the encoded string, or an empty array if the string is null or empty.</returns>
    /// <example>
    /// <code>
    /// byte[] utf8Bytes = "Hello".ToByteArray();
    /// byte[] asciiBytes = "Hello".ToByteArray(Encoding.ASCII);
    /// </code>
    /// </example>
    public static byte[] ToByteArray(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        if (value.IsNullOrEmpty())
            return [];

        return encoding.GetBytes(value);
    }

    /// <summary>
    /// Converts the string to bytes using the specified encoding and writes them to the destination buffer.
    /// This is an allocation-free overload for performance-critical scenarios.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="destination">The destination buffer to write the bytes to.</param>
    /// <param name="encoding">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if not specified.</param>
    /// <returns>The number of bytes written to the destination buffer.</returns>
    /// <remarks>
    /// Use <see cref="GetByteCount"/> to determine the required buffer size before calling this method.
    /// This method performs zero heap allocations.
    /// </remarks>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// Span&lt;byte&gt; buffer = stackalloc byte[text.GetByteCount()];
    /// int bytesWritten = text.ToByteArray(buffer);
    /// </code>
    /// </example>
    public static int ToByteArray(this string value, Span<byte> destination, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        if (value.IsNullOrEmpty())
            return 0;

        return encoding.GetBytes(value.AsSpan(), destination);
    }

    /// <summary>
    /// Gets the number of bytes required to encode the string using the specified encoding.
    /// </summary>
    /// <param name="value">The string to measure.</param>
    /// <param name="encoding">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if not specified.</param>
    /// <returns>The number of bytes required to encode the string, or 0 if the string is null or empty.</returns>
    /// <remarks>
    /// Use this method to allocate the correct buffer size before calling <see cref="ToByteArray(string, Span{byte}, Encoding?)"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// int byteCount = "Hello World".GetByteCount(); // 11 for UTF-8
    /// </code>
    /// </example>
    public static int GetByteCount(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        if (value.IsNullOrEmpty())
            return 0;

        return encoding.GetByteCount(value);
    }

    /// <summary>
    /// Concatenates the specified strings to the source string.
    /// </summary>
    /// <param name="value">The initial string value.</param>
    /// <param name="values">The strings to concatenate.</param>
    /// <returns>A string that consists of the source string followed by all the specified strings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null or empty.</exception>
    /// <remarks>
    /// This method uses <see cref="StringBuilder"/> internally for efficient concatenation,
    /// avoiding the allocation of intermediate string arrays.
    /// </remarks>
    /// <example>
    /// <code>
    /// string result = "Hello".Concat(new[] { " ", "World", "!" }); // "Hello World!"
    /// </code>
    /// </example>
    public static string Concat(this string value, IEnumerable<string> values)
    {
        value.GuardAgainstNullOrEmpty(nameof(value));

        if (values.IsNullOrEmpty())
            return value;

        var sb = new StringBuilder(value);
        foreach (var s in values)
        {
            sb.Append(s);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the string represents a valid GUID value.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns><c>true</c> if the string can be parsed as a GUID; otherwise, <c>false</c>.</returns>
    /// <remarks>This method accepts all standard GUID formats (N, D, B, P, X).</remarks>
    /// <example>
    /// <code>
    /// bool isGuid1 = "550e8400-e29b-41d4-a716-446655440000".IsGuidValue(); // true
    /// bool isGuid2 = "not-a-guid".IsGuidValue(); // false
    /// </code>
    /// </example>
    public static bool IsGuidValue(this string value)
        => Guid.TryParse(value, out _);

    /// <summary>
    /// Determines whether the string contains only digit characters (0-9).
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns><c>true</c> if the string is non-empty and contains only digits; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method is allocation-free, using <see cref="ReadOnlySpan{T}"/> for iteration.
    /// Note that this checks for digits only, not general numeric formats (no decimal points, signs, etc.).
    /// </remarks>
    /// <example>
    /// <code>
    /// bool isNumeric1 = "12345".IsNumeric(); // true
    /// bool isNumeric2 = "123.45".IsNumeric(); // false (contains decimal point)
    /// bool isNumeric3 = "-123".IsNumeric(); // false (contains minus sign)
    /// </code>
    /// </example>
    public static bool IsNumeric(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        foreach (var c in value.AsSpan())
        {
            if (!char.IsDigit(c))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the specified default value if the string is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="defaultValue">The default value to return if the string is null or empty. Defaults to an empty string.</param>
    /// <returns>The original value if not null/empty; otherwise, the <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// string result1 = ((string?)null).DefaultIfNull("N/A"); // "N/A"
    /// string result2 = "Hello".DefaultIfNull("N/A"); // "Hello"
    /// </code>
    /// </example>
    public static string DefaultIfNull(this string? value, string defaultValue = "")
        => value.IsNullOrEmpty() ? defaultValue : value;
}
