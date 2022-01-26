using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithEmptyValuesBehaviorTests
    {
        public static IEnumerable<object[]> FactoriesWithDefaultBehavior =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithEmptyFillingBehavior()},
                new object[] {MagicFactory.For<Customer>().WithBehavior(new FillWithEmptyValuesBehavior())}
            };

        [Theory]
        [MemberData(nameof(FactoriesWithDefaultBehavior))]
        public void It_should_fill_all_properties_with_empty_values(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be(string.Empty);
            customer.Address.Should().NotBeNull();
            customer.Address.ZipCode.Should().Be(string.Empty);
        }
        
        public static IEnumerable<object[]> FactoriesWithRecursionDisabled =>
            new List<object[]>
            {
                new object[] {new CustomerFactoryWithEmptyFillingBehaviorWithRecursionDisabled()},
                new object[] {
                    MagicFactory.For<Customer>()
                        .WithBehavior(new FillWithEmptyValuesBehavior(options => options.Recursive = false))
                }
            };
        
        [Theory]
        [MemberData(nameof(FactoriesWithRecursionDisabled))]
        public void It_should_fill_all_properties_with_empty_values_without_recursion(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be(string.Empty);
            customer.Address.Should().BeNull();
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
        
        private class CustomerFactoryWithEmptyFillingBehaviorWithRecursionDisabled : MagicFactory<Customer>
        {
            protected override void Customize(ICustomizeFactoryOptions<Customer> customization)
            {
                customization.SetDefaultBehavior(new FillWithEmptyValuesBehavior(options =>
                {
                    options.Recursive = false;
                }));
            }
        }
    }
}