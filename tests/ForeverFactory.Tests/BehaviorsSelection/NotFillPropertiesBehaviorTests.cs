using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Behaviors;
using ForeverFactory.Builders;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.BehaviorsSelection
{
    public class NotFillPropertiesBehaviorTests
    {
        public static IEnumerable<object[]> PersonFactoryWithDefaultBehaviorVariants =>
            new List<object[]>
            {
                new object[] {new PersonFactoryWithDefaultBehavior()},
                new object[] {MagicFactory.For<Person>()},
                new object[] {MagicFactory.For<Person>().WithBehavior(new DoNotFillBehavior())}
            };

        [Theory]
        [MemberData(nameof(PersonFactoryWithDefaultBehaviorVariants))]
        public void Default_filling_behavior_should_be_not_filling_properties_values(ISimpleFactory<Person> factory)
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
                customization.SetDefaultBehavior(new DoNotFillBehavior());
            }
        }
    }
}