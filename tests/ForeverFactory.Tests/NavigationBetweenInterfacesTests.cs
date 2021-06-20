using System.Linq;
using ForeverFactory.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace ForeverFactory.Tests
{
    public class NavigationBetweenInterfacesTests
    {
        [Fact]
        public void PlusOne_should_chain_previous_ManyBuilder_to_a_new_LinkedOneBuilder()
        {
            var persons = new PersonFactory()
                .Many(count: 5).With(x => x.Age = 99)
                .PlusOne().With(x => x.Age = 100)
                .Plus(count: 5).With(x => x.Age = 97)
                .PlusOne().With(x => x.Age = 101)
                .Build()
                .ToList();

            persons.Should().HaveCount(12);
            
            var firstTen = persons.Take(5);
            foreach (var person in firstTen)
            {
                person.Age.Should().Be(99);
            }

            var sixth = persons[5];
            sixth.Age.Should().Be(100);
            
            var nextFive = persons.Skip(6).Take(5);
            foreach (var person in nextFive)
            {
                person.Age.Should().Be(97);
            }
            
            var last = persons.Skip(11).First();
            last.Age.Should().Be(101);
        }
    }
}