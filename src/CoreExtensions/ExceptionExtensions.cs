using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Provides extension methods for guard clauses and validation that throw exceptions on invalid input.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Guard clauses are a common pattern for validating method parameters and enforcing preconditions.
    /// These extension methods provide a fluent API for common validation scenarios.
    /// </para>
    /// <para>
    /// All methods that validate non-null values use the <see cref="NotNullAttribute"/> to inform
    /// the compiler that after successful validation, the value is guaranteed to be non-null.
    /// </para>
    /// <para>
    /// Methods return the validated value to enable fluent chaining:
    /// <code>
    /// var user = GetUser()
    ///     .GuardAgainstNull(nameof(user))
    ///     .GuardAgainst(u => u.IsDisabled, "User is disabled");
    /// </code>
    /// </para>
    /// </remarks>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the value is null; otherwise, returns the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="parameterName">The name of the parameter being validated (for the exception message).</param>
        /// <returns>The value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        /// <remarks>
        /// The <see cref="NotNullAttribute"/> ensures the compiler knows the return value and the
        /// input value (if the method returns) are guaranteed to be non-null.
        /// </remarks>
        /// <example>
        /// <code>
        /// public void ProcessUser(User? user)
        /// {
        ///     user.GuardAgainstNull(nameof(user));
        ///     // user is now guaranteed non-null
        ///     Console.WriteLine(user.Name);
        /// }
        /// </code>
        /// </example>
        public static T GuardAgainstNull<T>([NotNull] this T? value, string parameterName)
            => value is not null ? value : throw new ArgumentNullException(parameterName);

        /// <summary>
        /// Throws the specified exception if the value is null; otherwise, returns the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of exception to throw. Must inherit from <see cref="Exception"/>.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="exception">The exception instance to throw if the value is null.</param>
        /// <returns>The value if it is not null.</returns>
        /// <exception cref="Exception">Thrown when <paramref name="value"/> is null (throws the provided exception).</exception>
        /// <remarks>
        /// Use this overload when you need to throw a custom exception type or provide additional context.
        /// </remarks>
        /// <example>
        /// <code>
        /// public User GetUser(int id)
        /// {
        ///     var user = repository.Find(id);
        ///     return user.GuardAgainstNull(new EntityNotFoundException($"User {id} not found"));
        /// }
        /// </code>
        /// </example>
        public static T GuardAgainstNull<T, TException>([NotNull] this T? value, TException exception)
            where TException : Exception
            => value is not null ? value : throw exception;


        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the predicate returns <c>true</c>; otherwise, returns the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="predicate">A function that returns <c>true</c> if the value is invalid.</param>
        /// <param name="message">The exception message to use if the predicate returns <c>true</c>.</param>
        /// <returns>The value if the predicate returns <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="predicate"/> returns <c>true</c>.</exception>
        /// <example>
        /// <code>
        /// public void SetAge(int age)
        /// {
        ///     age.GuardAgainst(a => a &lt; 0, "Age cannot be negative");
        ///     this.Age = age;
        /// }
        /// </code>
        /// </example>
        public static T GuardAgainst<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.GuardAgainstNull(nameof(predicate));

            return
            value.GuardAgainst(predicate, new ArgumentException(message));
        }

        /// <summary>
        /// Throws the specified exception if the predicate returns <c>true</c>; otherwise, returns the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of exception to throw. Must inherit from <see cref="Exception"/>.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="predicate">A function that returns <c>true</c> if the value is invalid.</param>
        /// <param name="exception">The exception instance to throw if the predicate returns <c>true</c>.</param>
        /// <returns>The value if the predicate returns <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="predicate"/> returns <c>true</c> (throws the provided exception).</exception>
        /// <example>
        /// <code>
        /// public void UpdateBalance(decimal amount)
        /// {
        ///     amount.GuardAgainst(
        ///         a => a &lt; 0,
        ///         new InvalidOperationException("Amount cannot be negative"));
        ///     Balance += amount;
        /// }
        /// </code>
        /// </example>
        public static T GuardAgainst<T, TException>(this T value, Func<T, bool> predicate, TException exception)
            where TException : Exception
        {
            predicate.GuardAgainstNull(nameof(predicate));

            if (predicate(value))
                throw exception;

            return value;
        }

        /// <summary>
        /// Throws an exception of type <typeparamref name="TException"/> if the predicate returns <c>true</c>; otherwise, returns the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of exception to throw. Must inherit from <see cref="Exception"/> and have a constructor accepting a string message.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="predicate">A function that returns <c>true</c> if the value is invalid.</param>
        /// <param name="message">The exception message to pass to the exception constructor.</param>
        /// <returns>The value if the predicate returns <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="predicate"/> returns <c>true</c> (throws an instance of <typeparamref name="TException"/>).</exception>
        /// <remarks>
        /// The exception is created using <see cref="Activator.CreateInstance(Type, object[])"/> with the message as the argument.
        /// The exception type must have a public constructor that accepts a single string parameter.
        /// </remarks>
        /// <example>
        /// <code>
        /// public void ProcessOrder(Order order)
        /// {
        ///     order.GuardAgainst&lt;Order, InvalidOperationException&gt;(
        ///         o => o.Status == OrderStatus.Cancelled,
        ///         "Cannot process a cancelled order");
        /// }
        /// </code>
        /// </example>
        public static T GuardAgainst<T, TException>(this T value, Func<T, bool> predicate, string message)
            where TException : Exception
        {
            predicate.GuardAgainstNull(nameof(predicate));

            return
            value.GuardAgainst(predicate, (TException)Activator.CreateInstance(typeof(TException), args: message)!);
        }


        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the string is null, empty, or consists only of white-space; otherwise, returns the string.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="parameterName">The name of the parameter being validated (for the exception message).</param>
        /// <param name="message">An optional custom message for the exception. If empty, a default message is used.</param>
        /// <returns>The string if it is not null, empty, or white-space.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null, empty, or consists only of white-space.</exception>
        /// <remarks>
        /// <para>
        /// This method uses <see cref="string.IsNullOrWhiteSpace"/> internally, so strings containing only
        /// whitespace characters (spaces, tabs, newlines) are also considered invalid.
        /// </para>
        /// <para>
        /// The <see cref="NotNullAttribute"/> ensures the compiler knows the return value is guaranteed to be non-null.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// public void SetName(string? name)
        /// {
        ///     Name = name.GuardAgainstNullOrEmpty(nameof(name), "Name is required");
        /// }
        /// </code>
        /// </example>
        public static string GuardAgainstNullOrEmpty([NotNull] this string? value, string parameterName, string message = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw message.IsNullOrEmpty()
                    ? new ArgumentNullException(parameterName)
                    : new ArgumentNullException(parameterName, message);
            }

            return value;
        }
    }
}
