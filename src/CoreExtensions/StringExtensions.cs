using System.Text;

namespace System
{
    /// <summary>
    /// string extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// performs the Equals call with StringComparison.InvariantCultureIgnoreCase.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCaseInvariant(this string src, string value)
        => src.GuardAgainstNullOrEmpty(nameof(src))
           .Equals(value, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>performs the Equals call with StringComparison.OrdinalIgnoreCase.</summary>
        /// <param name="src">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool EqualsIgnoreCaseOrdinal(this string src, string value)
       => src.GuardAgainstNullOrEmpty(nameof(src))
          .Equals(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// performs the Equals call with StringComparison.CurrentCultureIgnoreCase.
        /// </summary>
        /// <param name="src">src.</param>
        /// <param name="value">value.</param>
        /// <returns>true for equal false if not.</returns>
        public static bool EqualsIgnoreCaseCurrent(this string src, string value)
       => src.GuardAgainstNullOrEmpty(nameof(src))
          .Equals(value, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// returns an empty string if the given value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the given value or empty string if it is null.</returns>
        public static string EmptyIfNull(this string value)
            => value ?? string.Empty;

        /// <summary>
        /// Gets the string byte[] using the given encoding (defaults to UTF8)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            if (value.IsNullOrEmpty())
                return Array.Empty<byte>();

            return encoding.GetBytes(value);
        }

        /// <summary>
        /// Concats the given string vlaues to a single string value.
        /// </summary>
        /// <param name="value">the initial value</param>
        /// <param name="values">values that will be concatinated.</param>
        /// <returns>concatinated string.</returns>
        public static string Concat(this string value, IEnumerable<string> values)
        {
            value.GuardAgainstNullOrEmpty(nameof(value));

            if (values.IsNullOrEmpty())
                return value;

            var result = new List<string> { value };

            result.AddRange(values);

            return string.Concat(result);
        }

        /// <summary>
        /// checks wether the given string value a valid guid value or not.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public static bool IsGuidValue(this string value)
        => Guid.TryParse(value, out var g);

        public static bool IsNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return
            value.ToArray().All(c => Char.IsDigit(c));
        }

        /// <summary>
        /// returns the given defaultValue if the given value is null.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>returns the defaultvalue if value is null otherwise it returns the value.</returns>
        public static string DefaultIfNull(this string value, string defaultValue = "")
        => value.DefaultIfNull(defaultValue);
    }
}