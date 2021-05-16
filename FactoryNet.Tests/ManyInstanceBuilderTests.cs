using System.Linq;
using FactoryNet.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class ManyInstanceBuilderTests
    {
        private readonly PersonFactory _factory;

        public ManyInstanceBuilderTests()
        {
            _factory = new PersonFactory();
        }
            
        [Fact]
        public void It_produces_many_instances_with_default_configuration()
        {
            var persons = _factory
                .Many(count: 10)
                .Build()
                .ToList();

            persons.Should().HaveCount(10);
            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }
        }
            
        [Fact]
        public void It_produces_many_instances_with_overwritten_properties()
        {
            var persons = _factory
                .Many(count: 10)
                .With(x => x.Age = 19)
                .With(x => x.LastName = "Nobel")
                .Build()
                .ToList();

            persons.Should().HaveCount(10);
            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Nobel");
                person.Age.Should().Be(19);
            }
        }
    }
}