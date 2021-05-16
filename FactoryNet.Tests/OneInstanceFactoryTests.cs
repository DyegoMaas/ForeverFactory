using FactoryNet.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class OneInstanceFactoryTests
    {
        private readonly PersonFactory _factory;

        public OneInstanceFactoryTests()
        {
            _factory = new PersonFactory();
        }
            
        [Fact]
        public void A_custom_factory_should_produce_objects_with_values_configured_in_its_constructor()
        {
            var person = _factory.Build();

            person.FirstName.Should().Be("Albert");
            person.LastName.Should().Be("Einstein");
            person.Age.Should().Be(56);
        }

        [Fact]
        public void Individual_properties_can_be_overwritten()
        {
            var person = _factory
                .With(x => x.FirstName = "Guilherme")
                .With(x => x.Age = 19)
                .Build();

            person.FirstName.Should().Be("Guilherme");
            person.LastName.Should().Be("Einstein");
            person.Age.Should().Be(19);
        }
    }
}