using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories;

public class RecursiveTransformFactoryTests
{
    [Fact]
    public void It_should_build_a_function_that_recursively_sets_all_public_properties_and_fields_to_an_empty_value()
    {
        var transform = new FillWithEmptyValuesTransformFactory().GetTransform<ClassA>();

        var instanceOfA = new ClassA();
        transform.ApplyTo(instanceOfA);

        instanceOfA.PublicProperty.Should().Be(string.Empty);
        instanceOfA.PublicField.Should().Be(string.Empty);
        instanceOfA.NullableProperty.Should().Be(0);
        instanceOfA.NullableField.Should().Be(0);
        instanceOfA.B.Should().NotBeNull();

        var instanceOfB = instanceOfA.B;
        instanceOfB.Property.Should().Be(string.Empty);
        instanceOfB.PublicField.Should().Be(string.Empty);
        instanceOfB.NullableProperty.Should().Be(0);
        instanceOfB.NullableField.Should().Be(0);
        instanceOfB.C.Should().NotBeNull();
            
        var instanceOfC = instanceOfB.C;
        instanceOfC.PublicProperty.Should().Be(string.Empty);
        instanceOfC.PublicField.Should().Be(string.Empty);
        instanceOfC.NullableProperty.Should().Be(0);
        instanceOfC.NullableField.Should().Be(0);
    }

    private class ClassA
    {
        public string PublicProperty { get; set; }
        public string PublicField;
        public int? NullableProperty { get; set; }
        public int? NullableField;
        public ClassB B { get; set; }
    }

    private class ClassB
    {
        public string Property { get; set; }
        public string PublicField;
        public int? NullableProperty { get; set; }
        public int? NullableField;
        public ClassC C { get; set; }
    }
    
    private class ClassC
    {
        public string PublicProperty { get; set; }
        public string PublicField;
        public int? NullableProperty { get; set; }
        public int? NullableField;
    }
}