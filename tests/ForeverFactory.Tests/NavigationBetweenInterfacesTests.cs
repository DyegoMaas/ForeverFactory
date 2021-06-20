using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Builders;
using ForeverFactory.Tests.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests
{
    public class NavigationBetweenInterfacesTests
    {
        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void PlusOne_should_chain_previous_ManyBuilder_to_a_new_LinkedOneBuilder(ICustomizableFactory<Person> factory)
        {
            var persons = factory
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

        public static IEnumerable<object[]> FactoryInitializationVariants =>
            new List<object[]>
            {
                new object[] { new PersonFactory() },
                new object[] { MagicFactory.For<Person>() },
            };
    }
}