namespace System
{
    public static class ExceptionExtensions
    {
        [Obsolete("the name is misspelled, please use the method GuardAgainstNull instead")]
        public static T GuargAgainstNull<T>(this T value, string parameterName)
            => value.GuardAgainst(v => ReferenceEquals(v, null), new ArgumentNullException(parameterName));


        [Obsolete("the name is misspelled, please use the method GuardAgainst instead")]
        public static T GuargAgainst<T>(this T value, Func<T, bool> predicate, string message)
        => value.GuardAgainst(predicate, message);



        [Obsolete("the name is misspelled, please use the method GuardAgainst instead")]
        public static T GuargAgainst<T, TException>(this T value, Func<T, bool> predicate, TException exception)
            where TException : Exception
        => value.GuardAgainst(predicate, exception);


        public static T GuardAgainstNull<T>(this T value, string parameterName)
            => value
             .GuardAgainst(v => ReferenceEquals(v, null), new ArgumentNullException(parameterName));

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

            return
            value.GuardAgainst(predicate, exception);
        }

        public static string GuardAgainstNullOrEmpty(this string value, string parameterName)
        => value
           .GuardAgainst(v => string.IsNullOrWhiteSpace(v), new ArgumentNullException(parameterName));

    }
}