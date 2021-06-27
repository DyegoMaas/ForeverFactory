using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.FluentInterfaces;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Factories
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
        public void It_should_be_possible_to_chain_another_one_after_many(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .Many(2).With(x => x.Age = 99)
                .PlusOne().With(x => x.Age = 100)
                .Build()
                .ToList();

            persons.Should().HaveCount(3);

            persons[0].Age.Should().Be(99);
            persons[1].Age.Should().Be(99);
            persons[2].Age.Should().Be(100);
        }
        
        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void It_should_be_possible_to_chain_many_more_after_many(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .Many(2).With(x => x.Age = 99)
                .Plus(2).With(x => x.Age = 100)
                .Build()
                .ToList();

            persons.Should().HaveCount(4);

            persons[0].Age.Should().Be(99);
            persons[1].Age.Should().Be(99);
            persons[2].Age.Should().Be(100);
            persons[3].Age.Should().Be(100);
        }
        
        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void It_should_be_possible_to_chain_another_one_builder(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .One().With(x => x.Age = 99)
                .PlusOne().With(x => x.Age = 100)
                .Build()
                .ToList();

            persons.Should().HaveCount(2);

            persons[0].Age.Should().Be(99);
            persons[1].Age.Should().Be(100);
        }
        
        [Theory]
        [MemberData(nameof(FactoryInitializationVariants))]
        public void It_should_be_possible_to_chain_many_after_one(ISimpleFactory<Person> factory)
        {
            var persons = factory
                .One().With(x => x.Age = 99)
                .Plus(2).With(x => x.Age = 100)
                .Build()
                .ToList();

            persons.Should().HaveCount(3);

            persons[0].Age.Should().Be(99);
            persons[1].Age.Should().Be(100);
            persons[2].Age.Should().Be(100);
        }
    }
}