using System.Linq;
using FluentAssertions;
using ForeverFactory.Core;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using ForeverFactory.Tests.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Core
{
    public class GeneratorNodeTests
    {
        [Fact]
        public void It_should_produce_one_instance_if_no_target_count_is_specified()
        {
            var generatorNode = new GeneratorNode<Person>();

            var instances = generatorNode.ProduceInstances();

            instances.Should().HaveCount(1);
        }
            
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void It_should_produce_specified_number_of_instances(int targetCount)
        {
            var generatorNode = new GeneratorNode<Person>(targetCount);

            var instances = generatorNode.ProduceInstances();

            instances.Should().HaveCount(targetCount);
        }

        [Fact]
        public void It_should_accept_a_custom_constructor_function()
        {
            var generatorNode = new GeneratorNode<Person>(
                customConstructor: () => new Person {FirstName = "John", LastName = "Doe"}
            );

            var persons = generatorNode.ProduceInstances();

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("John");
                person.LastName.Should().Be("Doe");
            }
        }

        [Fact]
        public void It_should_apply_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(targetCount: 3);

            generatorNode.AddTransform(
                transform: new FuncTransform<Person, string>(x => x.FirstName = "Martha"),
                guard: new AlwaysApplyTransformGuardSpecification()
            );
                
            var persons = generatorNode.ProduceInstances();

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Martha");
            }
        }
            
        [Fact]
        public void It_should_apply_transforms_with_guards()
        {
            var generatorNode = new GeneratorNode<Person>(targetCount: 5, 
                customConstructor: () => new Person {FirstName = "Anne"}
            );
            generatorNode.AddTransform(
                transform: new FuncTransform<Person, string>(x => x.FirstName = "Martha"),
                guard: new ApplyTransformToFirstInstancesSpecification(countToApply: 2)
            );
            generatorNode.AddTransform(
                transform: new FuncTransform<Person, string>(x => x.FirstName = "Mirage"),
                guard: new ApplyTransformToLastInstancesSpecification(countToApply: 2, targetCount: 5)
            );
                
            var persons = generatorNode.ProduceInstances().ToArray();

            var firstNames = persons.Select(x => x.FirstName);
            firstNames.Should().BeEquivalentTo("Martha", "Martha", "Anne", "Mirage", "Mirage");
        }
            
        [Fact]
        public void It_should_apply_default_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(targetCount: 3);

            var transform1 = new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.FirstName = "Martha"));
            var transform2 = new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.LastName = "Kane"));
            var persons = generatorNode.ProduceInstances(defaultTransforms: new[] {transform1, transform2});

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Martha");
                person.LastName.Should().Be("Kane");
            }
        }

        [Fact]
        public void It_should_apply_default_transforms_before_normal_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(targetCount: 3);
            generatorNode.AddTransform(
                transform: new FuncTransform<Person, string>(x => x.FirstName = "Jonathan"),
                guard: new AlwaysApplyTransformGuardSpecification()
            );
                
            var transform1 = new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.FirstName = "Martha"));
            var transform2 = new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.LastName = "Kane"));
            var persons = generatorNode.ProduceInstances(defaultTransforms: new[] {transform1, transform2});

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Jonathan");
                person.LastName.Should().Be("Kane");
            }
        }
    }
}