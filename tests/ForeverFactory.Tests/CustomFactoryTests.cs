﻿using ForeverFactory.Tests.ExampleFactories;
using FluentAssertions;
using Xunit;

namespace ForeverFactory.Tests
{
    public class CustomFactoryTests
    {
        [Fact]
        public void Custom_factories_should_produce_instances_using_custom_constructor()
        {
            var factoryWithCustomConstructor = new ProductFactory();
            
            var product = factoryWithCustomConstructor.Build();

            product.Name.Should().Be("Nimbus 2000");
            product.Category.Should().Be("Brooms");
            product.Description.Should().Be("The best flight await you!");
        }
        
       
    }
}