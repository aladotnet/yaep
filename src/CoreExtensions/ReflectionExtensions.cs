using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Provides extension methods for reflection operations including type hierarchy inspection,
    /// assembly filtering, attribute checking, and referenced assembly loading.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides utilities for common reflection scenarios:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Checking if a type inherits from a generic base type</description></item>
    /// <item><description>Filtering types across assemblies with predicates</description></item>
    /// <item><description>Checking for custom attributes on types</description></item>
    /// <item><description>Determining if a type is static</description></item>
    /// <item><description>Loading referenced assemblies with caching</description></item>
    /// </list>
    /// <para>
    /// Results from <see cref="LoadReferencedAssemblies"/> are cached using <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// for thread-safety and to avoid repeated assembly loading.
    /// </para>
    /// </remarks>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Thread-safe cache for referenced assemblies, keyed by the source assembly.
        /// </summary>
        private static readonly ConcurrentDictionary<Assembly, ImmutableHashSet<Assembly>> ReferencedAssembliesCache = new();

        /// <summary>
        /// Determines whether the specified type is a subclass of a generic base type.
        /// </summary>
        /// <param name="subType">The type to check (the potential derived type).</param>
        /// <param name="genericBaseType">The generic type definition to check against (e.g., <c>typeof(List&lt;&gt;)</c>).</param>
        /// <returns>
        /// <c>true</c> if <paramref name="subType"/> inherits from <paramref name="genericBaseType"/>
        /// (including if it's the same generic type); otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method traverses the type hierarchy from <paramref name="subType"/> up to <see cref="object"/>,
        /// checking if any base type matches the generic type definition.
        /// </para>
        /// <para>
        /// Note that this method only checks base classes, not interfaces. Use
        /// <see cref="Type.GetInterfaces"/> to check for generic interface implementations.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Direct inheritance
        /// class StringList : List&lt;string&gt; { }
        /// typeof(StringList).IsSubClassOfGenericBase(typeof(List&lt;&gt;)); // true
        ///
        /// // Same generic type
        /// typeof(List&lt;int&gt;).IsSubClassOfGenericBase(typeof(List&lt;&gt;)); // true
        ///
        /// // Indirect inheritance
        /// class MyStringList : StringList { }
        /// typeof(MyStringList).IsSubClassOfGenericBase(typeof(List&lt;&gt;)); // true
        ///
        /// // Not a subclass
        /// typeof(Dictionary&lt;string, int&gt;).IsSubClassOfGenericBase(typeof(List&lt;&gt;)); // false
        /// </code>
        /// </example>
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
        /// Filters and returns types from the specified assemblies that match the given predicate.
        /// </summary>
        /// <param name="assemblies">The assemblies to search for types.</param>
        /// <param name="predicate">A function to test each type. Returns <c>true</c> to include the type in the result.</param>
        /// <returns>
        /// An enumerable of types from the assemblies that satisfy the predicate.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses <c>yield return</c> for lazy evaluation and manual iteration
        /// to avoid LINQ closure allocations.
        /// </para>
        /// <para>
        /// Types are retrieved using <see cref="Assembly.GetTypes"/> which may throw
        /// <see cref="ReflectionTypeLoadException"/> if some types cannot be loaded.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var assemblies = new[] { typeof(MyClass).Assembly };
        ///
        /// // Find all public classes with a specific naming convention
        /// var controllers = assemblies.TypesWhere(t =>
        ///     t.IsClass &amp;&amp; t.IsPublic &amp;&amp; t.Name.EndsWith("Controller"));
        ///
        /// // Find all types implementing a specific interface
        /// var services = assemblies.TypesWhere(t =>
        ///     typeof(IService).IsAssignableFrom(t) &amp;&amp; !t.IsInterface);
        /// </code>
        /// </example>
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
        /// Determines whether the specified type has a custom attribute of the given type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to check for. Must inherit from <see cref="Attribute"/>.</typeparam>
        /// <param name="type">The type to check for the attribute.</param>
        /// <returns>
        /// <c>true</c> if the type has the specified attribute; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="MemberInfo.GetCustomAttribute{T}()"/> which only checks
        /// for attributes directly applied to the type, not inherited attributes.
        /// </remarks>
        /// <example>
        /// <code>
        /// [Serializable]
        /// public class MyClass { }
        ///
        /// typeof(MyClass).HasCustomAttribute&lt;SerializableAttribute&gt;(); // true
        /// typeof(MyClass).HasCustomAttribute&lt;ObsoleteAttribute&gt;();     // false
        /// </code>
        /// </example>
        public static bool HasCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
            => type.GetCustomAttribute<TAttribute>() is not null;

        /// <summary>
        /// Determines whether the specified type is a static class.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the type is a static class; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// In .NET, static classes are represented as both <c>abstract</c> and <c>sealed</c>,
        /// which is how this method determines if a type is static.
        /// </para>
        /// <para>
        /// Note that this only applies to classes. Interfaces and other type kinds
        /// will return <c>false</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// public static class Utilities { }
        /// public abstract class BaseClass { }
        /// public sealed class FinalClass { }
        ///
        /// typeof(Utilities).IsStatic();   // true
        /// typeof(BaseClass).IsStatic();   // false (abstract but not sealed)
        /// typeof(FinalClass).IsStatic();  // false (sealed but not abstract)
        /// </code>
        /// </example>
        public static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

        /// <summary>
        /// Loads and returns all assemblies referenced by the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose references to load.</param>
        /// <returns>
        /// An <see cref="ImmutableHashSet{T}"/> containing all successfully loaded referenced assemblies.
        /// Assemblies that fail to load are silently skipped.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Results are cached per assembly using <see cref="ConcurrentDictionary{TKey, TValue}"/>
        /// to avoid repeated assembly loading on subsequent calls.
        /// </para>
        /// <para>
        /// Assemblies that cannot be loaded (e.g., missing dependencies, security restrictions)
        /// are silently skipped. This method does not throw exceptions for failed loads.
        /// </para>
        /// <para>
        /// The returned <see cref="ImmutableHashSet{T}"/> is thread-safe and can be safely
        /// shared across threads.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var assembly = typeof(MyClass).Assembly;
        /// var references = assembly.LoadReferencedAssemblies();
        ///
        /// foreach (var refAssembly in references)
        /// {
        ///     Console.WriteLine(refAssembly.GetName().Name);
        /// }
        ///
        /// // Subsequent calls return cached result
        /// var references2 = assembly.LoadReferencedAssemblies(); // Same instance
        /// </code>
        /// </example>
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
