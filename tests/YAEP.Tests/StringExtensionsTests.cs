using FluentAssertions;
using System;
using Xunit;

namespace YAEPTests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_Given_null_returns_true()
        {
            string value = null;

            value.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Given_none_null_returns_false()
        {
            var value = "Test";

            value.IsNullOrEmpty().Should().BeFalse();
        }

        [Fact]
        public void DefaultIfNull_Given_null_returns_defaultValue()
        {
            string value = null;
            var defaulValue = "default";
            value.DefaultIfNull(defaulValue).Should().Be(defaulValue);
        }

        [Fact]
        public void DefaultIfNull_Given_non_null_returns_value()
        {
            var value = "test";
            var defaulValue = "default";
            value.DefaultIfNull(defaulValue).Should().Be(value);
        }
    }
}
