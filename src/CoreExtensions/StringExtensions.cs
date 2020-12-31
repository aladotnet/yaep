using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool EqualsIgnoreCaseInvariant(this string src, string value)
        => src.GuardAgainstNullOrEmpty(nameof(src))
           .Equals(value, StringComparison.InvariantCultureIgnoreCase);

        public static bool EqualsIgnoreCaseOrdinal(this string src, string value)
       => src.GuardAgainstNullOrEmpty(nameof(src))
          .Equals(value, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsIgnoreCaseCurrent(this string src, string value)
       => src.GuardAgainstNullOrEmpty(nameof(src))
          .Equals(value, StringComparison.CurrentCultureIgnoreCase);

        public static bool ContainsIgnoreCase(this string src, string value,StringComparison comparer = StringComparison.OrdinalIgnoreCase)
        {
            return src.Contains(value);
        }

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

        public static string Concat(this string value,IEnumerable<string> values)
        {
            value.GuardAgainstNullOrEmpty(nameof(value));

            if (values.IsNullOrEmpty())
                return value;

            var result =  new string[] { value }.Union(values);

            return string.Concat(result);
        }

        public static bool IsGuidValue(this string value)
        => Guid.TryParse(value, out var g);
        
    }
}
