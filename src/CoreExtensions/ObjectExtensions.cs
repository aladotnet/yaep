using System;

namespace System
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj)
        {
            return ReferenceEquals( obj , null);
        }

        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }

        public static T DefaultIfNull<T>(this T value, T defaultValue)
        {
            if (value.IsNull())
                return defaultValue;

            return value;
        }

    }
}
