using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Exceptions extension methods.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Guards the against null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T GuardAgainstNull<T>([NotNull] this T? value, string parameterName)
            => value is not null ? value : throw new ArgumentNullException(parameterName);

        public static T GuardAgainstNull<T,TException>([NotNull] this T? value, TException exception)
            where TException : Exception
            => value is not null ? value : throw exception;


        /// <summary>
        /// Guards against the given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static T GuardAgainst<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.GuardAgainstNull(nameof(predicate));

            return
            value.GuardAgainst(predicate, new ArgumentException(message));
        }

        /// <summary>
        /// Guards against the given predicate and throws the given exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static T GuardAgainst<T, TException>(this T value, Func<T, bool> predicate, TException exception)
            where TException : Exception
        {
            predicate.GuardAgainstNull(nameof(predicate));

            if (predicate(value))
                throw exception;

            return value;
        }

        /// <summary>
        /// Guards against the given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static T GuardAgainst<T,TException>(this T value, Func<T, bool> predicate, string message)
            where TException : Exception
        {
            predicate.GuardAgainstNull(nameof(predicate));

            return
            value.GuardAgainst(predicate, (TException)Activator.CreateInstance(typeof(TException), args: message)!);
        }


        /// <summary>
        /// Guards the against null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
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
