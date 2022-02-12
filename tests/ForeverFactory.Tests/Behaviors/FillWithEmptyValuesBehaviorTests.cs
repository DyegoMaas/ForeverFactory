using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithEmptyValuesBehaviorTests
    {
        public static IEnumerable<object[]> FactoriesWithDefaultBehavior()
        {
            var customizedBehavior = new FillWithEmptyValuesBehavior();
            return new List<object[]>
            {
                new object[] { new CustomerFactory(customizedBehavior) },
                new object[] { MagicFactory.For<Customer>().WithBehavior(customizedBehavior) }
            };
        }

        [Theory]
        [MemberData(nameof(FactoriesWithDefaultBehavior))]
        public void It_should_fill_all_properties_with_empty_values(ISimpleFactory<Customer> factory)
        {
            var customer = factory.Build();

            customer.Name.Should().Be(string.Empty);
            customer.Address.Should().NotBeNull();
            customer.Address.ZipCode.Should().Be(string.Empty);
        }

        public static IEnumerable<object[]> FactoriesWithRecursionDisabled()
        {
            var customizedBehavior = new FillWithEmptyValuesBehavior(options => options.Recursive = false);
            return new List<object[]>
            {
                new object[] { new CustomerFactory(customizedBehavior) },
                new object[]
                {
                    MagicFactory.For<Customer>().WithBehavior(customizedBehavior)
                }
            };
        }

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

        private class CustomerFactory : MagicFactory<Customer>
        {
            private readonly FillWithEmptyValuesBehavior _behavior;

            public CustomerFactory(FillWithEmptyValuesBehavior behavior)
            {
                _behavior = behavior;
            }

            protected override void Customize(ICustomizeFactoryOptions<Customer> customization)
            {
                customization.SetDefaultBehavior(_behavior);
            }
        }
    }
}