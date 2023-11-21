using FluentAssertions;
using System;
using Xunit;

namespace YAEPTests
{
    public class ExceptionEtensionsTests
    {
        [Fact]
        public void GuardAgainst_given_a_true_predicate_throwes_an_exception()
        {
            var value = 42;

            Assert.Throws<ArgumentException>(() => value.GuardAgainst(v => v == 42, new ArgumentException()));
        }

        [Fact]
        public void GuardAgainst_given_a_false_predicate_returns_the_value()
        {
            var value = 42;
            var result = value.GuardAgainst(v => v == -1, new ArgumentException());

            result.Should().Be(42);
        }

        [Fact]
        public void GuardAgainstNull_given_a_null_throwes_ArgumentNullException()
        {
            Person person = null;

            Assert.Throws<ArgumentNullException>(() => person.GuardAgainstNull(nameof(person)));
        }

        [Fact]
        public void GuardAgainstNull_given_a_non_null_returns_the_value()
        {
            var person = new Person("Test", "Test");
            var result = person.GuardAgainstNull(nameof(person));

            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("Test");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void GuardAgainstNullOrEmpty_given_a_null_Or_Empty_or_whitespace_string_throwes_ArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => value.GuardAgainstNullOrEmpty(nameof(value)));
        }

        [Fact]
        public void GuardAgainstNullOrEmpty_given_a_non_null_returns_the_value()
        {
            var value = "Test";
            var result = value.GuardAgainstNullOrEmpty(nameof(value));

            result.Should().Be("Test");
        }


    }
}
