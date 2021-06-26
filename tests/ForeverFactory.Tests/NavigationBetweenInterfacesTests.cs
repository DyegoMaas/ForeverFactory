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
        public static IEnumerable<object[]> FactoryInitializationVariants =>
            new List<object[]>
            {
                new object[] {new PersonFactory()},
                new object[] {MagicFactory.For<Person>()}
            };

        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void Should_allow_navigation_starting_from_many(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .Many(5).With(x => x.Age = 99)
                .PlusOne().With(x => x.Age = 100)
                .Plus(5).With(x => x.Age = 97)
                .PlusOne().With(x => x.Age = 101)
                .Build()
                .ToList();

            persons.Should().HaveCount(12);

            var firstTen = persons.Take(5);
            foreach (var person in firstTen) person.Age.Should().Be(99);

            var sixth = persons[5];
            sixth.Age.Should().Be(100);

            var nextFive = persons.Skip(6).Take(5);
            foreach (var person in nextFive) person.Age.Should().Be(97);

            var last = persons.Skip(11).First();
            last.Age.Should().Be(101);
        }

        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void It_should_be_possible_to_explicitly_say_you_are_building_one_instance(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .One().With(x => x.Age = 99)
                .Plus(4).With(x => x.Age = 99)
                .PlusOne().With(x => x.Age = 100)
                .Plus(5).With(x => x.Age = 97)
                .PlusOne().With(x => x.Age = 101)
                .Build()
                .ToList();

            persons.Should().HaveCount(12);

            var firstTen = persons.Take(5);
            foreach (var person in firstTen) person.Age.Should().Be(99);

            var sixth = persons[5];
            sixth.Age.Should().Be(100);

            var nextFive = persons.Skip(6).Take(5);
            foreach (var person in nextFive) person.Age.Should().Be(97);

            var last = persons.Skip(11).First();
            last.Age.Should().Be(101);
        }
    }
}