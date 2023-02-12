using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace YAEP.Tests
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void IsSubclassOfGenericBase_Given_subtype_is_derived_from_genericBase_returns_true()
        {
            var subType = typeof(Derived);
            var baseType = typeof(Base<>);

            subType.IsSubclassOfGenericBase(baseType).Should().BeTrue();
        }

        class Base<T>
        {

        }

        class Derived : Base<string> { }
    }
}
