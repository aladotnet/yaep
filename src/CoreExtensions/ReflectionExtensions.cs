namespace System.Threading.Tasks
{
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
        public static bool IsSubclassOfGenericBase(this Type subType, Type genericBaseType)
        {
            while (subType.IsNotNull() && subType != typeof(object))
            {
                var cur = subType.IsGenericType ? subType.GetGenericTypeDefinition() : subType;
                if (genericBaseType == cur)
                {
                    return true;
                }
                subType = subType.BaseType;
            }
            return false;
        }
    }
}