using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithProperyNameBehaviorTests
    {
        public static IEnumerable<object[]> PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithPropertyNameFillingBehavior()},
                new object[] {MagicFactory.For<Customer>().WithBehavior(new FillWithProperyNameBehavior())}
            };

        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_with_property_names(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be("Name1");
            customer.Address.Should().NotBeNull();
            customer.Address.ZipCode.Should().Be("ZipCode1");
        }
        
        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_with_property_names_and_sequential_numbers(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Many(3).Build().ToArray();

            customer[0].Name.Should().Be("Name1");
            customer[1].Name.Should().Be("Name2");
            customer[2].Name.Should().Be("Name3");
        }
        
        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_recursively_with_property_names_and_sequential_numbers(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Many(3).Build().ToArray();

            customer[0].Address.ZipCode.Should().Be("ZipCode1");
            customer[1].Address.ZipCode.Should().Be("ZipCode2");
            customer[2].Address.ZipCode.Should().Be("ZipCode3");
        }

        public class Customer
        {
            public string Name { get; set; }
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
                customization.SetDefaultBehavior(new FillWithProperyNameBehavior());
            }
        }
    }
}