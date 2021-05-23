using ForeverFactory.Tests.ExampleFactories;
using FluentAssertions;
using ForeverFactory.Builders;
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
        public void It_should_link_to_a_LinkedManyBuilder_with_method_Plus()
        {
            var builder = MagicFactory.For<Product>();

            IManyBuilder<Product> newBuilder = builder.Plus(5);

            newBuilder.Should().BeOfType<LinkedManyBuilder<Product>>();
        }
        
        [Fact]
        public void It_should_link_to_a_LinkedOneBuilder_with_method_Plus()
        {
            var builder = MagicFactory.For<Product>();

            ILinkedOneBuilder<Product> newBuilder = builder.PlusOne();

            newBuilder.Should().BeOfType<LinkedOneBuilder<Product>>();
        }
    }
}