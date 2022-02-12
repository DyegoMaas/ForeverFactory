using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Behaviors;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Behaviors
{
    public class FillWithSequentialValuesBehaviorTests
    {
        public class DefaultConfigurationTests
        {
            public static IEnumerable<object[]> FactoriesWithDefaultBehavior()
            {
                var behavior = new FillWithSequentialValuesBehavior();
                return new List<object[]>
                {
                    new object[] { new CustomerFactory(behavior) },
                    new object[] { MagicFactory.For<Customer>().WithBehavior(behavior) }
                };
            }

            [Theory]
            [MemberData(nameof(FactoriesWithDefaultBehavior))]
            public void It_should_fill_all_objects_properties_with_sequential_numbers(ISimpleFactory<Customer> factory)
            {
                var customers = factory.Many(3).Build().ToArray();

                customers[0].Name.Should().Be("Name1");
                customers[0].Age.Should().Be(1);
                customers[0].Birthday.Should().Be(1.January(1753));
                customers[1].Name.Should().Be("Name2");
                customers[1].Age.Should().Be(2);
                customers[1].Birthday.Should().Be(2.January(1753));
                customers[2].Name.Should().Be("Name3");
                customers[2].Age.Should().Be(3);
                customers[2].Birthday.Should().Be(3.January(1753));
            }

            [Theory]
            [MemberData(nameof(FactoriesWithDefaultBehavior))]
            public void It_should_fill_all_properties_recursively(ISimpleFactory<Customer> factory)
            {
                var customer = factory.Many(3).Build().ToArray();

                customer[0].Address.ZipCode.Should().Be("ZipCode1");
                customer[1].Address.ZipCode.Should().Be("ZipCode2");
                customer[2].Address.ZipCode.Should().Be("ZipCode3");
            }
        }

        public class RecursionDisabledTests
        {
            [Theory]
            [MemberData(nameof(FactoriesWithRecursionDisabled))]
            public void It_should_fill_all_properties_with_empty_values_without_recursion(
                ISimpleFactory<Customer> factory)
            {
                var customer = factory.Build();

                customer.Name.Should().Be("Name1");
                customer.Age.Should().Be(1);
                customer.Birthday.Should().Be(1.January(1753));
                customer.Address.Should().BeNull();
            }

            public static IEnumerable<object[]> FactoriesWithRecursionDisabled()
            {
                var customizedBehavior = new FillWithSequentialValuesBehavior(options =>
                {
                    options.Recursive = false;
                });
                
                return new List<object[]>
                {
                    new object[] { new CustomerFactory(customizedBehavior) },
                    new object[] { MagicFactory.For<Customer>()
                        .WithBehavior(new FillWithSequentialValuesBehavior(options => 
                            options.Recursive = false)
                        )
                    }
                };
            }
        }

        public class CustomizedDateTimeGenerationTests
        {
            public static IEnumerable<object[]> FactoriesWithDefaultBehavior()
            {
                var customizedBehavior = new FillWithSequentialValuesBehavior(options =>
                {
                    options.DateTimeOptions = new DateTimeSequenceOptions
                    {
                        StartDate = 25.December(2020).At(22.Hours()),
                        DateTimeIncrements = DateTimeIncrements.Hours
                    };
                });
                
                return new List<object[]>
                {
                    new object[] {new CustomerFactory(customizedBehavior)},
                    new object[] {MagicFactory.For<Customer>().WithBehavior(customizedBehavior)}
                };
            }

            [Theory]
            [MemberData(nameof(FactoriesWithDefaultBehavior))]
            public void It_should_fill_all_objects_properties_with_sequential_numbers(ISimpleFactory<Customer> factory)
            {
                var customers = factory.Many(3).Build().ToArray();

                customers[0].Birthday.Should().Be(25.December(2020).At(22.Hours()));
                customers[1].Birthday.Should().Be(25.December(2020).At(23.Hours()));
                customers[2].Birthday.Should().Be(26.December(2020).At(0.Hours()));
            }
        }
        
        public class CustomerFactory : MagicFactory<Customer>
        {
            private readonly FillWithSequentialValuesBehavior _customizedBehavior;

            public CustomerFactory(FillWithSequentialValuesBehavior customizedBehavior)
            {
                _customizedBehavior = customizedBehavior;
            }

            protected override void Customize(ICustomizeFactoryOptions<Customer> customization)
            {
                customization.SetDefaultBehavior(_customizedBehavior);
            }
        }

        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime Birthday { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string ZipCode { get; set; }
        }
    }
}