using System.Collections.Immutable;
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

        public static IEnumerable<Type> TypesWhere(this IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        => assemblies.SelectMany(assembly => assembly.GetTypes())
                     .Where(predicate);

        public static bool HasCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
            => type.GetCustomAttribute<TAttribute>().IsNotNull();

        public static bool IsStatic(this Type type)=> type.IsAbstract && type.IsSealed;

        public static ImmutableHashSet<Assembly> LoadReferencedAssemblies(this  Assembly assembly)
        {
            var assemblyNames = assembly
                               .GetReferencedAssemblies()
                               .ToImmutableHashSet();

            return LoadAssemblies(assemblyNames);
        }

        private static ImmutableHashSet<Assembly> LoadAssemblies(ImmutableHashSet<AssemblyName> assemblyNames)
        {
            var assemblies = new HashSet<Assembly>();

            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    // Try to load the referenced assembly...
                    assemblies.Add(Assembly.Load(assemblyName));
                }
                catch
                {
                    // Failed to load assembly. Skip it.
                }
            }

            return assemblies.ToImmutableHashSet();
        }
    }
}
