using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Behaviors;
using ForeverFactory.Customizations.Global;
using Xunit;

namespace ForeverFactory.Tests.Behaviors.Configurations;

public class GlobalStaticConfigurationTests : IDisposable
{
    public GlobalStaticConfigurationTests()
    {
        ForeverFactoryGlobalSettings
            .UseBehavior(new FillWithSequentialValuesBehavior(options =>
            {
                options.DateTimeOptions = new DateTimeSequenceOptions
                {
                    DateTimeIncrements = DateTimeIncrements.Hours,
                    StartDate = 2.September(2020)
                };
                options.FillNullables = false;
                options.Recursive = false;
            }));
    }

    [Fact]
    public void globally_set_behaviors_should_be_used_in_new_factories()
    {
        var instances = MagicFactory.For<ClassA>().Many(2).Build().ToArray();
    
        instances[0].DateTimeProperty.Should().Be(2.September(2020));
        instances[0].NullableDateTimeProperty.Should().BeNull("FillNullables option is set to false");
        instances[0].B.Should().BeNull("Recursive option is set to false");
        instances[1].DateTimeProperty.Should().Be(2.September(2020).At(1.Hours()));
        instances[1].NullableDateTimeProperty.Should().BeNull("FillNullables option is set to false");
        instances[1].B.Should().BeNull("Recursive option is set to false");
    }
    
    [Fact]
    public void globally_set_behaviors_should_be_overridable()
    {
        var instance = MagicFactory.For<ClassA>()
            .WithBehavior(new DoNotFillBehavior())
            .Build();

        instance.DateTimeProperty.Should().Be(default);
        instance.NullableDateTimeProperty.Should().BeNull();
        instance.B.Should().BeNull();
    }

    public void Dispose()
    {
        ForeverFactoryGlobalSettings.ResetBehavior();
    }

    private class ClassA
    {
        public DateTime DateTimeProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }
        public ClassB B { get; set; }
    }

    private class ClassB
    {
    }
}