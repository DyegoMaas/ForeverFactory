using FactoryNet.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class DynamicFactoryTests
    {
        [Fact]
        public void Should_create_instance_with_default_values()
        {
            var factory = MagicFactory.For<Person>();
            
            var person = factory.Build();

            person.FirstName.Should().BeNull();
            person.LastName.Should().BeNull();
            person.Age.Should().Be(0);
        }
        
        [Fact]
        public void Should_allow_customization()
        {
            var factory = MagicFactory.For<Person>();
            
            var person = factory
                .With(x => x.FirstName = "Martha")
                .With(x => x.LastName = "Kent")
                .With(x => x.Age = 60)
                .Build();

            person.FirstName.Should().Be("Martha");
            person.LastName.Should().Be("Kent");
            person.Age.Should().Be(60);
        }
    }
}