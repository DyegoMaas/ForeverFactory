using FactoryNet.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class CustomConstructorTests
    {
        [Fact]
        public void Custom_factories_should_produce_instances_using_custom_constructor()
        {
            var factoryWithCustomConstructor = new ProductFactory();
            
            var product = factoryWithCustomConstructor.Build();

            product.Name.Should().Be("Nimbus 2000");
            product.Category.Should().Be("Brooms");
            product.Description.Should().Be("You will fly");
        }
        
        [Fact]
        public void Dynamic_factories_should_produce_instances_using_custom_constructor()
        {
             var product = MagicFactory.For<Product>()
                .UsingConstructor(() => new Product("MAG-7", "Shotgun"))
                .With(x => x.Description = "South Africa, 1995")
                .Build();

            product.Name.Should().Be("MAG-7");
            product.Category.Should().Be("Shotgun");
            product.Description.Should().Be("South Africa, 1995");
        }
    }
}