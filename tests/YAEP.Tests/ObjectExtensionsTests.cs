using FluentAssertions;
using System;
using Xunit;

namespace YAEPTests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void IsNull_given_a_Null_object_returns_true()
        {
            Person obj = null;

            var isNull = obj.IsNull();

            isNull.Should().BeTrue();
        }

        [Fact]
        public void IsNull_given_a_non_Null_object_returns_false()
        {
            var obj = new Person("Test", "Test");
            var isNull = obj.IsNull();

            isNull.Should().BeFalse();
        }

        [Fact]
        public void IsNotNull_given_a_Null_object_returns_false()
        {
            Person obj = null;

            var isNotNull = obj.IsNotNull();

            isNotNull.Should().BeFalse();
        }

        [Fact]
        public void IsNotNull_given_a_non_Null_object_returns_true()
        {
            var obj = new Person("Test", "Test");
            var isNotNull = obj.IsNotNull();

            isNotNull.Should().BeTrue();
        }

        [Fact]
        public void DefaultIfNull_given_a_null_reference_returnn_the_given_default()
        {
            Person obj = null;

            var result = obj.DefaultIfNull(new Person("-", "-"));

            result.Should().NotBeNull();
            result.FirstName.Should().Be("-");
            result.LastName.Should().Be("-");
        }

        [Fact]
        public void DefaultIfNull_given_a_non_null_reference_returns_the_same_value()
        {
            var obj = new Person("Test", "Test");

            var result = obj.DefaultIfNull(new Person("-", "-"));

            result.Should().NotBeNull();
            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("Test");
        }
    }


}
