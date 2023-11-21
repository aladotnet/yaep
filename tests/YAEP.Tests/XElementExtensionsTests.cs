using FluentAssertions;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace YAEPTests
{
    public class XElementExtensionsTests
    {
        [Fact]
        public void RootParent_returns_the_direct_ancestor()
        {
            var xml = @"<root><parent><child></child></parent></root>";

            var doc = XElement.Parse(xml);

            var child = doc.Descendants("child").Single();

            child.RootParent().Name.LocalName.Should().Be("root");
        }

        [Fact]
        public void RootParent_given_no_ancestors_returns_null()
        {
            var xml = @"<root><parent><child></child></parent></root>";

            var doc = XElement.Parse(xml);

            doc.RootParent().Should().BeNull();
        }

    }
}
