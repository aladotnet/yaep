namespace System
{
    public static class ExceptionExtensions
    {
        public static T GuardAgainstNull<T>(this T value, string parameterName)
            => !ReferenceEquals(value, null) ? value : throw new ArgumentNullException(parameterName);             

        public static T GuardAgainst<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.GuardAgainstNull(nameof(predicate));

            return
            value.GuardAgainst(predicate, new ArgumentException(message));
        }

        public static T GuardAgainst<T, TException>(this T value, Func<T, bool> predicate, TException exception)
            where TException : Exception
        {
            predicate.GuardAgainstNull(nameof(predicate));

            if (predicate(value))
                throw exception;
            
            return value;
        }

        public static string GuardAgainstNullOrEmpty(this string value, string parameterName)
        => value
           .GuardAgainst(v => string.IsNullOrWhiteSpace(v), new ArgumentNullException(parameterName));

    }
}