using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Builders;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Tests.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests
{
    public class MagicFactoryTests
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

        [Fact]
        public void Should_produce_an_instance_using_custom_constructor()
        {
            var product = MagicFactory.For<Product>()
                .UsingConstructor(() => new Product("MAG-7", "Shotgun"))
                .With(x => x.Description = "South Africa, 1995")
                .Build();

            product.Name.Should().Be("MAG-7");
            product.Category.Should().Be("Shotgun");
            product.Description.Should().Be("South Africa, 1995");
        }

        [Fact]
        public void Should_produce_multiple_instances_using_custom_constructor()
        {
            var products = MagicFactory.For<Product>()
                .UsingConstructor(() => new Product("MAG-7", "Shotgun"))
                .Many(2)
                .Plus(2)
                .Build();

            products.Should().HaveCount(4);
            foreach (var product in products)
            {
                product.Name.Should().Be("MAG-7");
                product.Category.Should().Be("Shotgun");
            }
        }
        
        [Fact]
        public void It_should_be_possible_to_explicitly_say_you_are_building_one_instance()
        {
            var product = new ProductFactory()
                .One()
                .With(x => x.Description = "Nimbus 2000")
                .Build();

            product.Description.Should().Be("Nimbus 2000");
        }

        [Fact]
        public void Transforms_are_applied_in_order_overriding_previous_ones()
        {
            var product = new ProductFactory()
                .With(x => x.Description = "Description 1")
                .With(x => x.Description = "Description 2")
                .Build();

            product.Description.Should().Be("Description 2");
        }
    }
}