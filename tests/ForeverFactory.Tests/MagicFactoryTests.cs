using ForeverFactory.Tests.ExampleFactories;
using FluentAssertions;
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
        public void Should_produce_instances_using_custom_constructor()
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
        public void Should_produce_instances_using_custom_constructor2()
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
    }
}