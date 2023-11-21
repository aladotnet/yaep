using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace YAEPTests
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void IsSubclassOfGenericBase_Given_subtype_is_derived_from_genericBase_returns_true()
        {
            var subType = typeof(Derived);
            var baseType = typeof(Base<>);

            subType.IsSubClassOfGenericBase(baseType).Should().BeTrue();
        }

        class Base<T>
        {

        }

        class Derived : Base<string> { }
    }
}
