using System.Collections.Concurrent;

namespace System
{
    /// <summary>
    /// enum extension methods.
    /// </summary>
    public static class EnumExtensions
    {
        // Cache for enum values to avoid repeated allocations
        private static readonly ConcurrentDictionary<Type, Array> ValuesCache = new();
        private static readonly ConcurrentDictionary<Type, string[]> NamesCache = new();
        private static readonly ConcurrentDictionary<Type, (string Name, int Value)[]> NameValuePairsCache = new();

        //todo: should be tested
        internal static TEnum ToEnum<TEnum>(this string value)
            where TEnum : struct
        {
            value.GuardAgainstNullOrEmpty(nameof(value));
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        /// <summary>
        /// Converts to enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string? value, TEnum defaultValue)
            where TEnum : struct
        {
            return Enum.TryParse(value, true, out TEnum result)
                ? result
                : defaultValue;
        }

        /// <summary>
        /// Gets the values. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Array GetValues<TEnum>(this TEnum value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            return ValuesCache.GetOrAdd(type, static t => Enum.GetValues(t));
        }

        /// <summary>
        /// Gets the names. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string[] GetNames<TEnum>(this TEnum value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            return NamesCache.GetOrAdd(type, static t => Enum.GetNames(t));
        }

        /// <summary>
        /// Converts to namevaluepaires. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static (string Name, int Value)[] ToNameValuePares<TEnum>(this TEnum value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            return NameValuePairsCache.GetOrAdd(type, static t =>
            {
                var names = Enum.GetNames(t);
                var values = Enum.GetValues(t);
                var result = new (string Name, int Value)[names.Length];

                for (var i = 0; i < names.Length; i++)
                {
                    result[i] = (names[i], (int)values.GetValue(i)!);
                }

                return result;
            });
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static int GetValue<TEnum>(this TEnum value, string name)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            name.GuardAgainstNullOrEmpty(nameof(name));

            var valueNames = value.ToNameValuePares();

            // Manual iteration to avoid LINQ allocation
            foreach (var (n, v) in valueNames)
            {
                if (n.EqualsIgnoreCaseOrdinal(name))
                    return v;
            }

            throw new InvalidOperationException($"No enum value found with name '{name}'");
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetName<TEnum>(this TEnum @this, int value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            var valueNames = @this.ToNameValuePares();

            // Manual iteration to avoid LINQ allocation
            foreach (var (n, v) in valueNames)
            {
                if (v == value)
                    return n;
            }

            throw new InvalidOperationException($"No enum name found for value '{value}'");
        }
    }
}
