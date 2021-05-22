using System.Linq;
using ForeverFactory.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace ForeverFactory.Tests
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
        
        [Fact]
        public void It_is_possible_to_build_a_sequence_of_objects_one_by_one()
        {
            var persons = _factory
                .With(x => x.FirstName = "Guilherme")
                .With(x => x.Age = 30)
                .PlusOne()
                .With(x => x.FirstName = "Dyego")
                .With(x => x.Age = 32)
                .Build()
                .ToArray();

            persons.Should().HaveCount(2);
            
            var guilherme = persons[0];
            guilherme.FirstName.Should().Be("Guilherme");
            guilherme.LastName.Should().Be("Einstein");
            guilherme.Age.Should().Be(30);
            
            var dyego = persons[1];
            dyego.FirstName.Should().Be("Dyego");
            dyego.LastName.Should().Be("Einstein");
            dyego.Age.Should().Be(32);
        }
        
        [Fact]
        public void It_is_possible_to_build_many_after_just_one()
        {
            var persons = _factory
                .With(x => x.FirstName = "Guilherme")
                .With(x => x.Age = 30)
                .Plus(10)
                .With(x => x.FirstName = "Dyego")
                .With(x => x.Age = 32)
                .Build()
                .ToArray();

            persons.Should().HaveCount(11);
            
            var guilherme = persons.First();
            guilherme.FirstName.Should().Be("Guilherme");
            guilherme.LastName.Should().Be("Einstein");
            guilherme.Age.Should().Be(30);
            
            var otherPersons = persons.Skip(1).ToArray();
            foreach (var person in otherPersons)
            {
                person.FirstName.Should().Be("Dyego");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(32);    
            }
        }
        
        [Fact]
        public void When_plus_is_called_from_a_MagicFactory_instance_it_chains_the_previous_default_configuration_to_the_next()
        {
            var persons = _factory
                .Plus(1)
                .Build()
                .ToArray();

            persons.Should().HaveCount(2);
        }
    }
}