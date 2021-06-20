using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Builders;
using ForeverFactory.Core.FactoryBehaviors;
using ForeverFactory.Customizations;
using Xunit;

namespace ForeverFactory.Tests.BehaviorsSelection
{
    public class FillWithEmptyValuesBehaviorTests
    {
        [Theory]
        [MemberData(nameof(PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior))]
        public void It_should_fill_all_properties_with_empty_values(
            ICustomizableFactory<Customer> factory)
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
                customization.SetPropertyFillBehavior(Behaviors.FillWithEmpty);
            }
        }

        public static IEnumerable<object[]> PersonFactoriesWithFillPropertiesWithEmptyValuesBehavior =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithEmptyFillingBehavior()},
                // new object[] {MagicFactory.For<Customer>().WithBehavior(Behaviors.FillWithEmpty)},
            };
    }
}