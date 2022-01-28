using System.Linq;
using FluentAssertions;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Guards;
using ForeverFactory.Generators.Transforms.Guards.Specifications;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Generators
{
    public class GeneratorNodeTests
    {
        [Fact]
        public void It_should_produce_one_instance_if_no_target_count_is_specified()
        {
            var generatorNode = new GeneratorNode<Person>();

            var instances = generatorNode.GenerateInstances();

            instances.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void It_should_produce_specified_number_of_instances(int targetCount)
        {
            var generatorNode = new GeneratorNode<Person>(targetCount);

            var instances = generatorNode.GenerateInstances();

            instances.Should().HaveCount(targetCount);
        }

        [Fact]
        public void It_should_accept_a_custom_constructor_function()
        {
            var generatorNode = new GeneratorNode<Person>(
                // customConstructor: () => new Person {FirstName = "John", LastName = "Doe"}
            );

            var persons = generatorNode.GenerateInstances(customConstructor: () => new Person {FirstName = "John", LastName = "Doe"});

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("John");
                person.LastName.Should().Be("Doe");
            }
        }

        [Fact]
        public void It_should_apply_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(3);

            generatorNode.AddTransform(
                new FuncTransform<Person, string>(x => x.FirstName = "Martha"),
                new AlwaysApplyTransformSpecification()
            );

            var persons = generatorNode.GenerateInstances();

            foreach (var person in persons) person.FirstName.Should().Be("Martha");
        }

        [Fact]
        public void It_should_apply_transforms_with_guards()
        {
            var generatorNode = new GeneratorNode<Person>(5);
            generatorNode.AddTransform(
                new FuncTransform<Person, string>(x => x.FirstName = "Martha"),
                new ApplyTransformToFirstInstancesSpecification(countToApply: 2, targetCount: 5)
            );
            generatorNode.AddTransform(
                new FuncTransform<Person, string>(x => x.FirstName = "Mirage"),
                new ApplyTransformToLastInstancesSpecification(countToApply: 2, targetCount: 5)
            );

            var persons = generatorNode
                .GenerateInstances(customConstructor: () => new Person {FirstName = "Anne"})
                .ToArray();

            var firstNames = persons.Select(x => x.FirstName);
            firstNames.Should().BeEquivalentTo("Martha", "Martha", "Anne", "Mirage", "Mirage");
        }

        [Fact]
        public void It_should_apply_default_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(3);

            var transform1 =
                new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.FirstName = "Martha"));
            var transform2 =
                new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.LastName = "Kane"));
            var persons = generatorNode.GenerateInstances(new[] {transform1, transform2});

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Martha");
                person.LastName.Should().Be("Kane");
            }
        }

        [Fact]
        public void It_should_apply_default_transforms_before_normal_transforms()
        {
            var generatorNode = new GeneratorNode<Person>(3);
            generatorNode.AddTransform(
                new FuncTransform<Person, string>(x => x.FirstName = "Jonathan"),
                new AlwaysApplyTransformSpecification()
            );

            var transform1 =
                new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.FirstName = "Martha"));
            var transform2 =
                new NotGuardedTransform<Person>(new FuncTransform<Person, string>(x => x.LastName = "Kane"));
            var persons = generatorNode.GenerateInstances(new[] {transform1, transform2});

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Jonathan");
                person.LastName.Should().Be("Kane");
            }
        }

        [Fact]
        public void It_should_return_the_target_count()
        {
            var generatorNode = new GeneratorNode<Person>(3);

            var targetCount = generatorNode.InstanceCount;

            targetCount.Should().Be(3);
        }

        // [Fact]
        // public void It_should_override_the_custom_constructor()
        // {
        //     var generatorNode = new GeneratorNode<Person>(1, () => new Person {Age = 10});
        //
        //     generatorNode.OverrideCustomConstructor(() => new Person {Age = 11});
        //
        //     var person = generatorNode.GenerateInstances().First();
        //     person.Age.Should().Be(11);
        // }
    }
}