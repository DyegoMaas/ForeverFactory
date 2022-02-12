using System;
using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories
{
    public class FillWithEmptyValuesTransformFactoryTests
    {
        [Fact]
        public void It_should_build_a_function_that_recursively_sets_all_properties_to_an_empty_value()
        {
            var transform = new FillWithEmptyValuesTransformFactory().GetTransform<ClassA>();

            var instanceOfA = new ClassA();
            transform.ApplyTo(instanceOfA);

            instanceOfA.StringProperty.Should().Be(string.Empty);
            instanceOfA.B.Should().NotBeNull();

            var instanceOfB = instanceOfA.B;
            instanceOfB.StringProperty.Should().Be(string.Empty);
            instanceOfB.C.Should().NotBeNull();
            
            var instanceOfC = instanceOfB.C;
            instanceOfC.StringProperty.Should().Be(string.Empty);
        }
        
        [Fact]
        public void It_should_build_a_function_that_initializes_all_supported_types()
        {
            var transform = new FillWithEmptyValuesTransformFactory().GetTransform<ClassA>();

            var instanceOfA = new ClassA();
            transform.ApplyTo(instanceOfA);

            instanceOfA.StringProperty.Should().Be(string.Empty);
            instanceOfA.DateTimeProperty.Should().Be(RecursiveTransformFactoryOptions.DefaultStartDate);
            instanceOfA.NullableByteProperty.Should().Be(0);
            instanceOfA.NullableShortProperty.Should().Be(0);
            instanceOfA.NullableUShortProperty.Should().Be(0);
            instanceOfA.NullableIntProperty.Should().Be(0);
            instanceOfA.NullableUIntProperty.Should().Be(0);
            instanceOfA.NullableLongProperty.Should().Be(0);
            instanceOfA.NullableULongProperty.Should().Be(0);
            instanceOfA.NullableFloatProperty.Should().Be(0f);
            instanceOfA.NullableDoubleProperty.Should().Be(0d);
            instanceOfA.NullableDecimalProperty.Should().Be(0m);
            instanceOfA.NullableDateTimeProperty.Should().Be(RecursiveTransformFactoryOptions.DefaultStartDate);
        }

        private class ClassA
        {
            public string StringProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public byte? NullableByteProperty { get; set; }
            public short? NullableShortProperty { get; set; }
            public ushort? NullableUShortProperty { get; set; }
            public int? NullableIntProperty { get; set; }
            public uint? NullableUIntProperty { get; set; }
            public long? NullableLongProperty { get; set; }
            public ulong? NullableULongProperty { get; set; }
            public float? NullableFloatProperty { get; set; }
            public double? NullableDoubleProperty { get; set; }
            public decimal? NullableDecimalProperty { get; set; }
            public DateTime? NullableDateTimeProperty { get; set; }
            public ClassB B { get; set; }
        }

        private class ClassB
        {
            public string StringProperty { get; set; }
            public ClassC C { get; set; }
        }
        
        private class ClassC
        {
            public string StringProperty { get; set; }
        }
        
        [Fact]
        public void Should_disable_recursive_fill()
        {
            var factoryWithRecursionDisabled = new FillWithEmptyValuesTransformFactory(
                new RecursiveTransformFactoryOptions {
                    EnableRecursiveInstantiation = false
                }
            );

            var transform = factoryWithRecursionDisabled.GetTransform<ClassA>();
            var instance = new ClassA();
            transform.ApplyTo(instance);

            instance.StringProperty.Should().Be(string.Empty);
            instance.B.Should().Be(null, "should not recursively fill when it is disable via options");
        }
    }
}