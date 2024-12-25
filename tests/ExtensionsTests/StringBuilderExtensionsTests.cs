using FluentAssertions;
using System.Text;
using Xunit;

namespace ExtensionsTests
{
    public class StringBuilderExtensionsTests
    {
        [Fact]
        public void AppendIf_Appends_Text_If_preficate_returns_true()
        {
            var sb = new StringBuilder();
            var text = "test-text";

            sb.AppendIf(() => true, text);

            sb.ToString().Should().Contain(text);
        }
    }
}
