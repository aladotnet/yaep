using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        // Cache for loaded assemblies to avoid repeated loading
        private static readonly ConcurrentDictionary<Assembly, ImmutableHashSet<Assembly>> ReferencedAssembliesCache = new();

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
            var currentSubType = subType;
            while (currentSubType is not null && currentSubType != typeof(object))
            {
                var current = currentSubType.IsGenericType ? currentSubType.GetGenericTypeDefinition() : currentSubType;
                if (genericBaseType == current)
                {
                    return true;
                }

                currentSubType = currentSubType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Filters types from assemblies based on a predicate.
        /// </summary>
        public static IEnumerable<Type> TypesWhere(this IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            // Manual iteration to avoid LINQ closure allocation for simple cases
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (predicate(type))
                        yield return type;
                }
            }
        }

        /// <summary>
        /// Determines whether the type has the specified custom attribute.
        /// </summary>
        public static bool HasCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
            => type.GetCustomAttribute<TAttribute>() is not null;

        /// <summary>
        /// Determines whether the type is static.
        /// </summary>
        public static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

        /// <summary>
        /// Loads all referenced assemblies. Results are cached per assembly.
        /// </summary>
        public static ImmutableHashSet<Assembly> LoadReferencedAssemblies(this Assembly assembly)
        {
            return ReferencedAssembliesCache.GetOrAdd(assembly, static asm =>
            {
                var assemblyNames = asm.GetReferencedAssemblies();
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
            });
        }
    }
}
