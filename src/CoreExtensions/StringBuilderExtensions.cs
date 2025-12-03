using System.Diagnostics.CodeAnalysis;

namespace System.Text
{
    /// <summary>
    /// Provides extension methods for <see cref="StringBuilder"/> operations with conditional execution support.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides conditional variants of common <see cref="StringBuilder"/> operations
    /// that only execute when a predicate returns <c>true</c>. This enables fluent, conditional
    /// string building without breaking the method chain.
    /// </para>
    /// <para>
    /// All conditional methods return the <see cref="StringBuilder"/> instance to enable method chaining.
    /// </para>
    /// </remarks>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the specified text to the <see cref="StringBuilder"/> only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="predicate">A function that determines whether to append the text.</param>
        /// <param name="text">The text to append.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder("Hello");
        /// sb.AppendIf(() => includeWorld, " World");
        /// // sb contains "Hello World" if includeWorld is true, otherwise just "Hello"
        /// </code>
        /// </example>
        public static StringBuilder AppendIf([NotNull] this StringBuilder builder, [NotNull] Func<bool> predicate, string text)
            => predicate
                .GuardAgainstNull(nameof(predicate))
                .Invoke() ? builder.Append(text)
                          : builder;

        /// <summary>
        /// Appends the specified text followed by a line terminator only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="predicate">A function that determines whether to append the line.</param>
        /// <param name="text">The text to append before the line terminator.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder();
        /// sb.AppendLineIf(() => showHeader, "Header")
        ///   .AppendLine("Content");
        /// </code>
        /// </example>
        public static StringBuilder AppendLineIf(this StringBuilder builder, Func<bool> predicate, string text)
            => predicate
                .GuardAgainstNull(nameof(predicate))
                .Invoke() ? builder.AppendLine(text)
                          : builder;

        /// <summary>
        /// Appends a line terminator only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="predicate">A function that determines whether to append the line terminator.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        public static StringBuilder AppendLineIf(this StringBuilder builder, Func<bool> predicate)
            => predicate
                .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendLine()
                         : builder;

        /// <summary>
        /// Appends the string representations of the values separated by a character only if the predicate returns <c>true</c>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the values collection.</typeparam>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="predicate">A function that determines whether to append the joined values.</param>
        /// <param name="separator">The character to use as a separator.</param>
        /// <param name="values">The values to join and append.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder();
        /// var items = new[] { "a", "b", "c" };
        /// sb.AppendJoinIf(() => items.Length > 0, ',', items);
        /// // sb contains "a,b,c"
        /// </code>
        /// </example>
        public static StringBuilder AppendJoinIf<T>(this StringBuilder builder, Func<bool> predicate, char separator, IEnumerable<T> values)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendJoin<T>(separator, values)
                         : builder;

        /// <summary>
        /// Appends the string representations of the values separated by a string only if the predicate returns <c>true</c>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the values collection.</typeparam>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="predicate">A function that determines whether to append the joined values.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">The values to join and append.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder();
        /// var numbers = new[] { 1, 2, 3 };
        /// sb.AppendJoinIf(() => true, " - ", numbers);
        /// // sb contains "1 - 2 - 3"
        /// </code>
        /// </example>
        public static StringBuilder AppendJoinIf<T>(this StringBuilder builder, Func<bool> predicate, string separator, IEnumerable<T> values)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendJoin<T>(separator, values)
                         : builder;

        /// <summary>
        /// Clears the <see cref="StringBuilder"/> only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to clear.</param>
        /// <param name="predicate">A function that determines whether to clear the builder.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder("Some content");
        /// sb.ClearIf(() => shouldReset);
        /// </code>
        /// </example>
        public static StringBuilder ClearIf(this StringBuilder builder, Func<bool> predicate)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Clear()
                         : builder;

        /// <summary>
        /// Removes a range of characters from the <see cref="StringBuilder"/> only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to modify.</param>
        /// <param name="predicate">A function that determines whether to remove the characters.</param>
        /// <param name="startIndex">The zero-based position where removal begins.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder("Hello World");
        /// sb.RemoveIf(() => removeWorld, 5, 6); // Removes " World"
        /// </code>
        /// </example>
        public static StringBuilder RemoveIf(this StringBuilder builder, Func<bool> predicate, int startIndex, int length)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Remove(startIndex, length)
                         : builder;

        /// <summary>
        /// Replaces all occurrences of a string only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to modify.</param>
        /// <param name="predicate">A function that determines whether to perform the replacement.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder("Hello World");
        /// sb.ReplaceIf(() => shouldReplace, "World", "Universe");
        /// // sb contains "Hello Universe" if shouldReplace is true
        /// </code>
        /// </example>
        public static StringBuilder ReplaceIf(this StringBuilder builder, Func<bool> predicate, string oldValue, string newValue)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Replace(oldValue, newValue)
                         : builder;

        /// <summary>
        /// Replaces occurrences of a string within a specified range only if the predicate returns <c>true</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to modify.</param>
        /// <param name="predicate">A function that determines whether to perform the replacement.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring.</param>
        /// <returns>The <see cref="StringBuilder"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <example>
        /// <code>
        /// var sb = new StringBuilder("Hello World World");
        /// sb.ReplaceIf(() => true, "World", "Universe", 0, 11);
        /// // Only replaces first "World": "Hello Universe World"
        /// </code>
        /// </example>
        public static StringBuilder ReplaceIf(this StringBuilder builder, Func<bool> predicate, string oldValue, string newValue, int startIndex, int count)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Replace(oldValue, newValue, startIndex, count)
                         : builder;

        /// <summary>
        /// Creates a new <see cref="StringBuilder"/> initialized with the specified string value.
        /// </summary>
        /// <param name="value">The string to use as the initial value.</param>
        /// <returns>A new <see cref="StringBuilder"/> containing the specified value.</returns>
        /// <remarks>
        /// This is a convenience method that provides a fluent way to create a <see cref="StringBuilder"/>
        /// from a string.
        /// </remarks>
        /// <example>
        /// <code>
        /// var sb = "Hello".ToStringBuilder()
        ///     .Append(" World")
        ///     .AppendLine("!");
        /// </code>
        /// </example>
        public static StringBuilder ToStringBuilder(this string value)
            => new StringBuilder(value);
    }
}
