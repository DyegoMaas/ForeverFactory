using FluentAssertions;
using ForeverFactory.Generators.Transforms.Factories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Factories
{
    public class FillWithPropertyNameTransformFactoryTests
    {
        [Fact]
        public void It_should_build_a_function_that_recursively_sets_all_properties_to_the_name_of_the_property()
        {
            var transform = new FillWithPropertyNameTransformFactory().GetTransformers<ClassA>();

            var instanceOfA = new ClassA();
            transform.ApplyTo(instanceOfA);

            instanceOfA.PropertyX.Should().Be("PropertyX1");
            instanceOfA.B.Should().NotBeNull();

            var instanceOfB = instanceOfA.B;
            instanceOfB.PropertyY.Should().Be("PropertyY1");
            instanceOfB.C.Should().NotBeNull();
            
            var instanceOfC = instanceOfB.C;
            instanceOfC.PropertyZ.Should().Be("PropertyZ1");
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
    }
}