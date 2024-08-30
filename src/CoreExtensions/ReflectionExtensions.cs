using System.Reflection;

namespace System
{
    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Determines whether [is subclass of generic base] [the specified generic base type].
        /// </summary>
        /// <param name="subType">the type of the derived class.</param>
        /// <param name="genericBaseType">the Type of the generic base class.</param>
        /// <returns>
        ///   <c>true</c> if [is subclass of generic base] [the specified generic base type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSubClassOfGenericBase(this Type subType, Type genericBaseType)
        {
            Type? currentSubType = subType;
            while (currentSubType.IsNotNull() && currentSubType != typeof(object))
            {
                var current = currentSubType!.IsGenericType ? currentSubType.GetGenericTypeDefinition() : currentSubType;
                if (genericBaseType == current)
                {
                    return true;
                }

                currentSubType = currentSubType.BaseType;
            }
            return false;
        }

        public static bool HasCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
            => type.GetCustomAttribute<TAttribute>().IsNotNull();
    }
}
