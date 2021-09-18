using System;
using System.Threading.Tasks;

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

        public static T Next<T>(this T @this, Func<T> action)
        {
            return
            action();
        }

        public static TResult Next<TInput, TResult>(this TInput state, Func<TInput, TResult> action)
        {
            return
            action(state);
        }

        public static Task<TResult> NextAsync<TInput, TResult>(this TInput state, Func<TInput, Task<TResult>> action)
        {
            return
            action(state);
        }

        public static Task<T> NextAsync<T>(this T @this, Func<Task<T>> action)
        {
            return
            action();
        }
    }
}
