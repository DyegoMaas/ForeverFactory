﻿using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Builders;
using ForeverFactory.Customizations;
using ForeverFactory.Tests.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.BehaviorsSelection
{
    public class NotFillPropertiesBehaviorTests
    {
        [Theory]
        [MemberData(nameof(PersonFactoryWithDefaultBehaviorVariants))]
        public void Default_filling_behavior_should_be_not_filling_properties_values(
            ICustomizableFactory<Person> factory)
        {
            var person = factory.Build();

            person.Age.Should().Be(default);
            person.FirstName.Should().Be(default);
            person.LastName.Should().Be(default);
        }

        private class PersonFactoryWithDefaultBehavior : MagicFactory<Person>
        {
            protected override void Customize(ICustomizeFactoryOptions<Person> customization)
            {
            }
        }

        public static IEnumerable<object[]> PersonFactoryWithDefaultBehaviorVariants =>
            new List<object[]>
            {
                new object[] {new PersonFactoryWithDefaultBehavior()},
                new object[] {MagicFactory.For<Person>()},
            };
    }
}