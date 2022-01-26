using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithSequentialValuesBehaviorTests
    {
        public static IEnumerable<object[]> PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithPropertyNameFillingBehavior()},
                new object[] {MagicFactory.For<Customer>().WithBehavior(new FillWithSequentialValuesBehavior())}
            };

        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_properties_with_sequential_values(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be("Name1");
            customer.Age.Should().Be(1);
            customer.Address.Should().NotBeNull();
            customer.Address.ZipCode.Should().Be("ZipCode1");
        }
        
        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_objects_properties_with_sequential_numbers(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Many(3).Build().ToArray();

            customer[0].Name.Should().Be("Name1");
            customer[0].Age.Should().Be(1);
            customer[1].Name.Should().Be("Name2");
            customer[1].Age.Should().Be(2);
            customer[2].Name.Should().Be("Name3");
            customer[2].Age.Should().Be(3);
        }
        
        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_recursively(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Many(3).Build().ToArray();

            customer[0].Address.ZipCode.Should().Be("ZipCode1");
            customer[1].Address.ZipCode.Should().Be("ZipCode2");
            customer[2].Address.ZipCode.Should().Be("ZipCode3");
        }

        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string ZipCode { get; set; }
        }

        private class CustomerFactoryWithPropertyNameFillingBehavior : MagicFactory<Customer>
        {
            protected override void Customize(ICustomizeFactoryOptions<Customer> customization)
            {
                customization.SetDefaultBehavior(new FillWithSequentialValuesBehavior());
            }
        }
    }
}