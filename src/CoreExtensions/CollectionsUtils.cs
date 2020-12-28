using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionsUtils
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list) => list.IsNull() || !list.Any();

        public static bool IsNotEmpty<T>(this IEnumerable<T> list) => !list.IsNull() && list.Any();

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items) 
            => items?.ToList()?.AsReadOnly() 
             ?? Enumerable.Empty<T>()
                .ToList().AsReadOnly();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }
    }
}
