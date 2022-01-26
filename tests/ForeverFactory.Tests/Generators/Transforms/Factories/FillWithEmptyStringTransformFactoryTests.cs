using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories
{
    public class TransformFactoryTests
    {
        [Fact]
        public void It_should_build_a_function_that_recursively_sets_all_properties_to_an_empty_value()
        {
            var transform = new FillWithEmptyStringTransformFactory().GetTransform<ClassA>();

            var instanceOfA = new ClassA();
            transform.ApplyTo(instanceOfA);

            instanceOfA.PropertyX.Should().Be(string.Empty);
            instanceOfA.B.Should().NotBeNull();

            var instanceOfB = instanceOfA.B;
            instanceOfB.PropertyY.Should().Be(string.Empty);
            instanceOfB.C.Should().NotBeNull();
            
            var instanceOfC = instanceOfB.C;
            instanceOfC.PropertyZ.Should().Be(string.Empty);
        }

        private class ClassA
        {
            public string PropertyX { get; set; }
            public ClassB B { get; set; }
        }

        private class ClassB
        {
            public string PropertyY { get; set; }
            public ClassC C { get; set; }
        }
        
        private class ClassC
        {
            public string PropertyZ { get; set; }
        }
        
        [Fact]
        public void Should_disable_recursive_fill()
        {
            var factoryWithRecursionDisabled = new FillWithEmptyStringTransformFactory(
                new RecursiveTransformFactoryOptions {
                    EnableRecursiveInstantiation = false
                }
            );

            var transform = factoryWithRecursionDisabled.GetTransform<ClassA>();
            var instance = new ClassA();
            transform.ApplyTo(instance);

            instance.PropertyX.Should().Be(string.Empty);
            instance.B.Should().Be(null, "should not recursively fill when it is disable via options");
        }
    }
}