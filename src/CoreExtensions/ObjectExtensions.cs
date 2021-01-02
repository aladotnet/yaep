using System;

namespace System
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj)
            where T : class
        {
            return ReferenceEquals( obj , null);
        }

        public static bool IsNotNull<T>(this T obj)
            where T : class
        {
            return !obj.IsNull();
        }

        public static T DefaultIfNull<T>(this T value, T defaultValue)
            where T:class
        {
            if (value.IsNull())
                return defaultValue;

            return value;
        }

    }
}
