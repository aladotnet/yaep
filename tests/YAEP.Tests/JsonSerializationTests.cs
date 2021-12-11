using Xunit;
using System.Text.Json;
using FluentAssertions;

namespace YAEP.Tests
{
    public class JsonSerializationTests
    {
        [Fact]
        public void ToJson_given_a_value_returns_a_json_serialization_string()
        {
            Person obj = new Person("Test", "Test");

            var json = obj.ToJson();

            var expected = JsonSerializer.Serialize(obj);

            json.Should().Be(expected);
        }



        [Fact]
        public void ToJson_given_a_json_serialization_string_returns_the_deserlized_type_instance()
        {
            var expected = new Person("Test", "Test");
            var json = JsonSerializer.Serialize(expected);
            
            var actual = json.JsonDeserialize<Person>();

            actual.FirstName.Should().Be(expected.FirstName);
            actual.LastName.Should().Be(expected.LastName);
        }
    }

   
}
