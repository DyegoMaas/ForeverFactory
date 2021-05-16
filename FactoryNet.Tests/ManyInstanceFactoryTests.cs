using System;
using System.Linq;
using FactoryNet.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class ManyInstanceFactoryTests
    {
        private readonly PersonFactory _factory;

        public ManyInstanceFactoryTests()
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

        [Fact]
        public void Should_throw_an_argument_exception_for_first_count_bigger_than_total_size()
        {
            Action invalidConfigurationBuild = () => _factory
                .Many(count: 10)
                .WithFirst(count: 11, x => x.Age = 19)
                .Build();

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void Should_throw_an_argument_exception_for_last_count_bigger_than_total_size()
        {
            Action invalidConfigurationBuild = () => _factory
                .Many(count: 10)
                .WithLast(count: 11, x => x.Age = 19)
                .Build();

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void WithFirst_applies_transformation_only_over_the_first_x_instances_produced()
        {
            var persons = _factory
                .Many(count: 10)
                .WithFirst(count: 2, x => x.Age = 19)
                .WithFirst(count: 2, x => x.LastName = "Nobel")
                .Build()
                .ToList();

            persons.Should().HaveCount(10);
            var firstTwo = persons.Take(2);
            foreach (var person in firstTwo)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Nobel");
                person.Age.Should().Be(19);
            }
            
            var lastEight = persons.Skip(2);
            foreach (var person in lastEight)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }
        }
        
        [Fact]
        public void WithLast_applies_transformation_only_over_the_last_x_instances_produced()
        {
            var persons = _factory
                .Many(count: 10)
                .WithLast(count: 2, x => x.Age = 19)
                .WithLast(count: 2, x => x.LastName = "Nobel")
                .Build()
                .ToList();

            persons.Should().HaveCount(10);
  
            var firstEigth = persons.Take(8);
            foreach (var person in firstEigth)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }
            
            var lastTwo = persons.Skip(8);
            foreach (var person in lastTwo)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Nobel");
                person.Age.Should().Be(19);
            }
        }
        
        [Fact]
        public void It_should_generate_multiple_sets_of_configurable_instances()
        {
            var persons = _factory
                .Many(count: 10)
                .WithLast(count: 2, x => x.Age = 19)
                .WithLast(count: 2, x => x.LastName = "Nobel")
                .Plus(count: 5)
                .WithFirst(count: 3, x => x.Age = 100)
                .With(x => x.FirstName = "Anna")
                .Build()
                .ToList();

            persons.Should().HaveCount(15);
  
            var firstEight = persons.Take(8);
            foreach (var person in firstEight)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }
            
            var nextTwo = persons.Skip(8).Take(2);
            foreach (var person in nextTwo)
            {
                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Nobel");
                person.Age.Should().Be(19);
            }
            
            var nextThree = persons.Skip(10).Take(3);
            foreach (var person in nextThree)
            {
                person.FirstName.Should().Be("Anna");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(100);
            }
            
            nextTwo = persons.TakeLast(2);
            foreach (var person in nextTwo)
            {
                person.FirstName.Should().Be("Anna");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }
        }
    }
}