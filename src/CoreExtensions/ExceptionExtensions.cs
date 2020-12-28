namespace System
{
    public static class ExceptionExtensions
    {
        public static T GuargAgainstNull<T>(this T value, string parameterName) 
            =>value
             .GuargAgainst(v => ReferenceEquals(v, null), new ArgumentNullException(parameterName));
        

        public static T GuargAgainst<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.GuargAgainstNull(nameof(predicate));

            return 
            value.GuargAgainst(predicate, new ArgumentException(message));
        }

        public static T GuargAgainst<T,TException>(this T value, Func<T, bool> predicate, TException exception)
            where TException : Exception
        {
            predicate.GuargAgainstNull(nameof(predicate));

            return
            value.GuargAgainst(predicate, exception);
        }

        public static string GuardAgainstNullOrEmpty(this string value, string parameterName)
        => value
           .GuargAgainst(v=> string.IsNullOrWhiteSpace(v), new ArgumentNullException(parameterName));

        


    }
}
