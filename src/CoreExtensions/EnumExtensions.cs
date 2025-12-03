using System.Collections.Concurrent;

namespace System
{
    /// <summary>
    /// Provides extension methods for enum operations including parsing, value retrieval, and name-value pair conversions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All methods that retrieve enum metadata (values, names, name-value pairs) use internal caching
    /// via <see cref="ConcurrentDictionary{TKey, TValue}"/> to avoid repeated allocations.
    /// The cache is thread-safe and results are computed only once per enum type.
    /// </para>
    /// <para>
    /// Methods use manual iteration instead of LINQ to avoid closure allocations.
    /// </para>
    /// </remarks>
    public static class EnumExtensions
    {
        /// <summary>
        /// Thread-safe cache for enum values arrays, keyed by enum type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Array> ValuesCache = new();

        /// <summary>
        /// Thread-safe cache for enum names arrays, keyed by enum type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string[]> NamesCache = new();

        /// <summary>
        /// Thread-safe cache for enum name-value pair arrays, keyed by enum type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, (string Name, int Value)[]> NameValuePairsCache = new();

        /// <summary>
        /// Parses a string value to the specified enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to parse to. Must be a struct.</typeparam>
        /// <param name="value">The string representation of the enum value.</param>
        /// <returns>The parsed enum value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not a valid member of <typeparamref name="TEnum"/>.</exception>
        /// <remarks>
        /// This method performs case-insensitive parsing.
        /// This is an internal method; use <see cref="ToEnum{TEnum}(string?, TEnum)"/> for public API with fallback support.
        /// </remarks>
        internal static TEnum ToEnum<TEnum>(this string value)
            where TEnum : struct
        {
            value.GuardAgainstNullOrEmpty(nameof(value));
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        /// <summary>
        /// Attempts to parse a string value to the specified enum type, returning a default value on failure.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to parse to. Must be a struct.</typeparam>
        /// <param name="value">The string representation of the enum value, or null.</param>
        /// <param name="defaultValue">The value to return if parsing fails.</param>
        /// <returns>
        /// The parsed enum value if successful; otherwise, <paramref name="defaultValue"/>.
        /// </returns>
        /// <remarks>
        /// This method performs case-insensitive parsing and never throws exceptions for invalid input.
        /// </remarks>
        /// <example>
        /// <code>
        /// var status = "active".ToEnum(Status.Unknown);     // Returns Status.Active
        /// var invalid = "invalid".ToEnum(Status.Unknown);   // Returns Status.Unknown
        /// string? nullStr = null;
        /// var fromNull = nullStr.ToEnum(Status.Unknown);    // Returns Status.Unknown
        /// </code>
        /// </example>
        public static TEnum ToEnum<TEnum>(this string? value, TEnum defaultValue)
            where TEnum : struct
        {
            return Enum.TryParse(value, true, out TEnum result)
                ? result
                : defaultValue;
        }

        /// <summary>
        /// Gets all values of the enum type. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type. Must be a struct.</typeparam>
        /// <param name="value">Any value of the enum type (used for type inference).</param>
        /// <returns>An <see cref="Array"/> containing all values of the enum type.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an enum type.</exception>
        /// <remarks>
        /// <para>
        /// Results are cached in a thread-safe manner using <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// Subsequent calls for the same enum type return the cached array without allocation.
        /// </para>
        /// <para>
        /// The returned array should not be modified as it is shared across all callers.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var values = DayOfWeek.Monday.GetValues();
        /// foreach (DayOfWeek day in values)
        /// {
        ///     Console.WriteLine(day);
        /// }
        /// </code>
        /// </example>
        public static Array GetValues<TEnum>(this TEnum value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            return ValuesCache.GetOrAdd(type, static t => Enum.GetValues(t));
        }

        /// <summary>
        /// Gets all names of the enum type. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type. Must be a struct.</typeparam>
        /// <param name="value">Any value of the enum type (used for type inference).</param>
        /// <returns>A string array containing all names of the enum type.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an enum type.</exception>
        /// <remarks>
        /// <para>
        /// Results are cached in a thread-safe manner. The returned array should not be modified.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var names = DayOfWeek.Monday.GetNames();
        /// // names = ["Sunday", "Monday", "Tuesday", ...]
        /// </code>
        /// </example>
        public static string[] GetNames<TEnum>(this TEnum value)
            where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException($"the given value [{value}] is not an enum");

            return NamesCache.GetOrAdd(type, static t => Enum.GetNames(t));
        }

        /// <summary>
        /// Gets all name-value pairs of the enum type. Results are cached per enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type. Must be a struct.</typeparam>
        /// <param name="value">Any value of the enum type (used for type inference).</param>
        /// <returns>An array of tuples containing the name and integer value of each enum member.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an enum type.</exception>
        /// <remarks>
        /// <para>
        /// Results are cached in a thread-safe manner. This method is useful for building
        /// UI elements like dropdown lists or for serialization scenarios.
        /// </para>
        /// <para>
        /// Values are cast to <see cref="int"/>. For enums with underlying types larger than int,
        /// use alternative approaches.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pairs = Status.Active.ToNameValuePares();
        /// foreach (var (name, value) in pairs)
        /// {
        ///     Console.WriteLine($"{name} = {value}");
        /// }
        /// // Output: Active = 0, Inactive = 1, Pending = 2
        /// </code>
        /// </example>
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
        /// Gets the integer value of an enum member by its name.
        /// </summary>
        /// <typeparam name="TEnum">The enum type. Must be a struct.</typeparam>
        /// <param name="value">Any value of the enum type (used for type inference).</param>
        /// <param name="name">The name of the enum member to look up.</param>
        /// <returns>The integer value corresponding to the specified name.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an enum type.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no enum member with the specified name exists.</exception>
        /// <remarks>
        /// <para>
        /// Name comparison is case-insensitive using ordinal comparison.
        /// Uses cached name-value pairs to avoid repeated enum reflection.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// int value = DayOfWeek.Monday.GetValue("Friday"); // Returns 5
        /// int value2 = DayOfWeek.Monday.GetValue("FRIDAY"); // Returns 5 (case-insensitive)
        /// </code>
        /// </example>
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
        /// Gets the name of an enum member by its integer value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type. Must be a struct.</typeparam>
        /// <param name="this">Any value of the enum type (used for type inference).</param>
        /// <param name="value">The integer value to look up.</param>
        /// <returns>The name corresponding to the specified integer value.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an enum type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no enum member with the specified value exists.</exception>
        /// <remarks>
        /// Uses cached name-value pairs to avoid repeated enum reflection.
        /// If multiple enum members have the same value, the first one (in declaration order) is returned.
        /// </remarks>
        /// <example>
        /// <code>
        /// string name = DayOfWeek.Monday.GetName(5); // Returns "Friday"
        /// </code>
        /// </example>
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
