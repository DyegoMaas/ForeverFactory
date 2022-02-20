using System;
using FluentAssertions;
using ForeverFactory.Behaviors;
using Xunit;

namespace ForeverFactory.Tests.Factories;

public class LayeredTransformApplicationTests
{
    [Fact]
    public void should_apply_behavior_first()
    {
        var instance = new SomeClassFactoryWithNoOverridenProperty()
            .WithBehavior(new FillWithEmptyValuesBehavior())
            .Build();

        instance.Name.Should().Be(string.Empty);
    }
    
    [Fact]
    public void factory_customization_should_override_behavior_transforms()
    {
        var instance = new SomeClassFactoryWithOverridenProperty()
            .WithBehavior(new FillWithEmptyValuesBehavior())
            .Build();

        instance.Name.Should().Be("John Doe");
    }
    
    [Fact]
    public void scenario_customization_should_override_factory_transforms()
    {
        var instance = new SomeClassFactoryWithOverridenProperty()
            .WithBehavior(new FillWithEmptyValuesBehavior())
            .With(x => x.Name = "New Name")
            .Build();

        instance.Name.Should().Be("New Name");
    }
    
    public class SomeClassFactoryWithNoOverridenProperty : MagicFactory<SomeClass>
    {
        protected override void Customize(ICustomizeFactoryOptions<SomeClass> customization)
        {
        }
    }
    
    public class SomeClassFactoryWithOverridenProperty : MagicFactory<SomeClass>
    {
        protected override void Customize(ICustomizeFactoryOptions<SomeClass> customization)
        {
            customization.Set(x => x.Name = "John Doe");
        }
    }
    
    public class SomeClass
    {
        public string Name { get; set; }
    }
}