using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories;

public class RecursiveTransformFactoryTests
{
    [Fact]
    public void It_should_build_a_function_that_recursively_sets_all_public_properties_and_fields_to_an_empty_value()
    {
        var transform = new FillWithEmptyStringTransformFactory().GetTransform<ClassA>();

        var instanceOfA = new ClassA();
        transform.ApplyTo(instanceOfA);

        instanceOfA.PublicProperty.Should().Be(string.Empty);
        instanceOfA.PublicField.Should().Be(string.Empty);
        instanceOfA.B.Should().NotBeNull();

        var instanceOfB = instanceOfA.B;
        instanceOfB.PropertyY.Should().Be(string.Empty);
        instanceOfB.PublicField.Should().Be(string.Empty);
        instanceOfB.C.Should().NotBeNull();
            
        var instanceOfC = instanceOfB.C;
        instanceOfC.PropertyZ.Should().Be(string.Empty);
        instanceOfC.PublicField.Should().Be(string.Empty);
    }

    private class ClassA
    {
        public string PublicProperty { get; set; }
        public string PublicField;
        public ClassB B { get; set; }
    }

    private class ClassB
    {
        public string PropertyY { get; set; }
        public string PublicField;
        public ClassC C { get; set; }
    }
    
    private class ClassC
    {
        public string PropertyZ { get; set; }
        public string PublicField;
    }
}