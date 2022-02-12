using System;
using System.Collections.Generic;
using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories;

public class RecursiveTransformFactoryTests
{
    public class RecursionTests
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

    public class NullableFillingDisabledTests
    {
        public static IEnumerable<object[]> FactoriesWithNullablesDisabled()
        {
            var options = new RecursiveTransformFactoryOptions { FillNullables = false };
            return new List<object[]>
            {
                new object[] { new FillWithEmptyValuesTransformFactory(options) },
                new object[] { new FillWithSequentialValuesTransformFactory(options) }
            };
        }

        [Theory]
        [MemberData(nameof(FactoriesWithNullablesDisabled))]
        internal void Should_disable_filling_nullable_fields_and_properties(BaseRecursiveTransformFactory factory)
        {
            var transform = factory.GetTransform<ClassWithNullableProperties>();
            var instance = new ClassWithNullableProperties();
            transform.ApplyTo(instance);
            
            instance.NullableByteProperty.Should().BeNull();
            instance.NullableShortProperty.Should().BeNull();
            instance.NullableUShortProperty.Should().BeNull();
            instance.NullableIntProperty.Should().BeNull();
            instance.NullableUIntProperty.Should().BeNull();
            instance.NullableLongProperty.Should().BeNull();
            instance.NullableULongProperty.Should().BeNull();
            instance.NullableFloatProperty.Should().BeNull();
            instance.NullableDoubleProperty.Should().BeNull();
            instance.NullableDecimalProperty.Should().BeNull();
            instance.NullableDateTimeProperty.Should().BeNull();
        }
        
        private class ClassWithNullableProperties
        {
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
        }
    }
}