using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithEmptyValuesBehaviorTests
    {
        public static IEnumerable<object[]> PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithEmptyFillingBehavior()},
                new object[] {MagicFactory.For<Customer>().WithBehavior(new FillWithEmptyValuesBehavior())}
            };

        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_with_empty_values(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be(string.Empty);
            customer.Address.Should().NotBeNull();
            customer.Address.ZipCode.Should().Be(string.Empty);
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

        private class CustomerFactoryWithEmptyFillingBehavior : MagicFactory<Customer>
        {
            protected override void Customize(ICustomizeFactoryOptions<Customer> customization)
            {
                customization.SetDefaultBehavior(new FillWithEmptyValuesBehavior());
            }
        }
    }
}