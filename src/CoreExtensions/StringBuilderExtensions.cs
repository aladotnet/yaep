namespace System.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder builder, Func<bool> predicate, string text)
            => predicate
                .GuardAgainstNull(nameof(predicate))
                .Invoke() ? builder.Append(text)
                          : builder;

        public static StringBuilder AppendLineIf(this StringBuilder builder, Func<bool> predicate, string text)
            => predicate
                .GuardAgainstNull(nameof(predicate))
                .Invoke() ? builder.AppendLine(text)
                          : builder;

        public static StringBuilder AppendLineIf(this StringBuilder builder, Func<bool> predicate)
            => predicate
                .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendLine()
                         : builder;

        public static StringBuilder AppendJoinIf<T>(this StringBuilder builder, Func<bool> predicate, char separator, IEnumerable<T> values)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendJoin<T>(separator, values)
                         : builder;

        public static StringBuilder AppendJoinIf<T>(this StringBuilder builder, Func<bool> predicate, string separator, IEnumerable<T> values)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.AppendJoin<T>(separator, values)
                         : builder;
        public static StringBuilder ClearIf(this StringBuilder builder, Func<bool> predicate)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Clear()
                         : builder;

        public static StringBuilder RemoveIf(this StringBuilder builder, Func<bool> predicate,int startIndex, int length)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Remove(startIndex,length)
                         : builder;

        public static StringBuilder ReplaceIf(this StringBuilder builder, Func<bool> predicate, string oldValue, string newValue)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Replace(oldValue, newValue)
                         : builder;

        public static StringBuilder ReplaceIf(this StringBuilder builder, Func<bool> predicate, string oldValue, string newValue,int startIndex, int count)
           => predicate
               .GuardAgainstNull(nameof(predicate))
               .Invoke() ? builder.Replace(oldValue, newValue,startIndex,count)
                         : builder;

        public static StringBuilder ToStringBuilder(this string value)
            => new StringBuilder(value);
    }
}
