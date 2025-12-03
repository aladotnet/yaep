using FluentAssertions;
using System.Reflection;

namespace ExtensionsTests;

/// <summary>
/// Tests for ReflectionExtensions methods.
/// </summary>
public class ReflectionExtensionsTests
{
    #region IsSubClassOfGenericBase

    /// <summary>
    /// Path: Type is direct subclass of generic base -> base type matches -> returns true.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenDirectSubclass_ReturnsTrue()
    {
        var result = typeof(StringList).IsSubClassOfGenericBase(typeof(List<>));

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Type is indirect subclass (grandchild) of generic base -> traverses hierarchy -> returns true.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenIndirectSubclass_ReturnsTrue()
    {
        var result = typeof(CustomStringList).IsSubClassOfGenericBase(typeof(List<>));

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Type is not subclass of generic base -> no match in hierarchy -> returns false.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenNonSubclass_ReturnsFalse()
    {
        var result = typeof(string).IsSubClassOfGenericBase(typeof(List<>));

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Type is the generic base itself -> matches itself -> returns true.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenSameGenericType_ReturnsTrue()
    {
        var result = typeof(List<int>).IsSubClassOfGenericBase(typeof(List<>));

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Type implements generic interface but not base -> no base match -> returns false.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenTypeWithGenericInterface_ReturnsFalse()
    {
        // Dictionary implements IEnumerable<> but doesn't inherit from List<>
        var result = typeof(Dictionary<string, int>).IsSubClassOfGenericBase(typeof(List<>));

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Custom generic base type -> type derives from it -> returns true.
    /// </summary>
    [Fact]
    public void IsSubClassOfGenericBase_GivenCustomGenericBase_ReturnsTrue()
    {
        var result = typeof(Derived).IsSubClassOfGenericBase(typeof(Base<>));

        result.Should().BeTrue();
    }

    #endregion

    #region TypesWhere

    /// <summary>
    /// Path: Assemblies with matching types -> filters by predicate -> returns matching types.
    /// </summary>
    [Fact]
    public void TypesWhere_GivenPredicateMatchingTypes_ReturnsFilteredTypes()
    {
        var assemblies = new[] { typeof(ReflectionExtensionsTests).Assembly };

        var result = assemblies.TypesWhere(t => t.Name.StartsWith("TestClass") && t.IsClass);

        result.Should().Contain(typeof(TestClass));
    }

    /// <summary>
    /// Path: Assemblies with no matching types -> predicate matches nothing -> returns empty.
    /// </summary>
    [Fact]
    public void TypesWhere_GivenPredicateMatchingNothing_ReturnsEmpty()
    {
        var assemblies = new[] { typeof(ReflectionExtensionsTests).Assembly };

        var result = assemblies.TypesWhere(t => t.Name == "NonExistentType12345");

        result.Should().BeEmpty();
    }

    #endregion

    #region HasCustomAttribute

    /// <summary>
    /// Path: Type has the specified attribute -> GetCustomAttribute returns non-null -> returns true.
    /// </summary>
    [Fact]
    public void HasCustomAttribute_GivenTypeWithAttribute_ReturnsTrue()
    {
        var result = typeof(TestClassWithAttribute).HasCustomAttribute<SerializableAttribute>();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Type does not have the specified attribute -> GetCustomAttribute returns null -> returns false.
    /// </summary>
    [Fact]
    public void HasCustomAttribute_GivenTypeWithoutAttribute_ReturnsFalse()
    {
        var result = typeof(TestClass).HasCustomAttribute<SerializableAttribute>();

        result.Should().BeFalse();
    }

    #endregion

    #region IsStatic

    /// <summary>
    /// Path: Static class -> IsAbstract && IsSealed -> returns true.
    /// </summary>
    [Fact]
    public void IsStatic_GivenStaticClass_ReturnsTrue()
    {
        var result = typeof(StaticTestClass).IsStatic();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Non-static class -> not (IsAbstract && IsSealed) -> returns false.
    /// </summary>
    [Fact]
    public void IsStatic_GivenNonStaticClass_ReturnsFalse()
    {
        var result = typeof(TestClass).IsStatic();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Abstract class -> IsAbstract but not IsSealed -> returns false.
    /// </summary>
    [Fact]
    public void IsStatic_GivenAbstractClass_ReturnsFalse()
    {
        var result = typeof(AbstractTestClass).IsStatic();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Sealed class -> IsSealed but not IsAbstract -> returns false.
    /// </summary>
    [Fact]
    public void IsStatic_GivenSealedClass_ReturnsFalse()
    {
        var result = typeof(SealedTestClass).IsStatic();

        result.Should().BeFalse();
    }

    #endregion

    #region LoadReferencedAssemblies

    /// <summary>
    /// Path: Assembly with references -> loads referenced assemblies -> returns non-empty set.
    /// </summary>
    [Fact]
    public void LoadReferencedAssemblies_GivenAssemblyWithReferences_ReturnsLoadedAssemblies()
    {
        var assembly = typeof(ReflectionExtensionsTests).Assembly;

        var result = assembly.LoadReferencedAssemblies();

        result.Should().NotBeEmpty();
        // Should contain at least System.Runtime or similar core assemblies
        result.Should().Contain(a => a.GetName().Name!.StartsWith("System") ||
                                      a.GetName().Name!.StartsWith("FluentAssertions") ||
                                      a.GetName().Name!.StartsWith("xunit"));
    }

    /// <summary>
    /// Path: Assembly references that can't be loaded -> catches exception -> skips failed loads.
    /// </summary>
    [Fact]
    public void LoadReferencedAssemblies_GivenFailedLoads_SkipsFailedAssemblies()
    {
        var assembly = typeof(ReflectionExtensionsTests).Assembly;

        // Should not throw even if some assemblies fail to load
        var act = () => assembly.LoadReferencedAssemblies();

        act.Should().NotThrow();
    }

    #endregion

    #region Test Helper Classes

    private class StringList : List<string> { }

    private class CustomStringList : StringList { }

    private class Base<T> { }

    private class Derived : Base<string> { }

    public class TestClass { }

    [Serializable]
    public class TestClassWithAttribute { }

    public static class StaticTestClass { }

    public abstract class AbstractTestClass { }

    public sealed class SealedTestClass { }

    #endregion
}
