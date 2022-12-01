using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace YAEP.Tests
{
    public class ExpressionsExtensionsTests
    {
        [Fact]
        public void ToPropertyExpression()
        {
            var p = new Person("Test", "Test");

            var exp = p.ToPropertyExpression("FirstName");
            var name = exp.GetName();

            name.Should().Be("FirstName");
        }

        [Fact]
        public void ToPropertyExpression_given_a_filedname_throws_an_exception()
        {
            var p = new Person("Test", "Test");

            Assert.Throws<ArgumentException>( ()=> p.ToPropertyExpression<Person, string>("ID"));
        }

        [Fact]
        public void ToPropertyExpression_given_a_methodname_throws_an_exception()
        {
            var p = new Person("Test", "Test");

            Assert.Throws<ArgumentException>(() => p.ToPropertyExpression<Person, string>("ToString"));
        }

        [Fact]
        public void GetName_given_a_property_expression_returns_the_name()
        {
            Expression<Func<Person, string>> exp = p => p.FirstName;
            var name = exp.GetName();

            name.Should().Be("FirstName");
        }

        [Fact]
        public void GetName_given_a_field_expression_returns_the_field_name()
        {
            Expression<Func<Person, int>> exp = p => p.ID;
            var name = exp.GetName();

            name.Should().Be("ID");
        }

        [Fact]
        public void GetName_given_a_Method_trows_an_exception()
        {
            Expression<Func<Person, string>> exp = p => p.ToString();
            Assert.Throws<InvalidCastException>(() => exp.GetName());
        }


    }
}
