using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Core;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using ForeverFactory.Tests.ExampleFactories;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace ForeverFactory.Tests.CoreV2
{
    public class CoreV2Tests
    {
        [Fact]
        public void It_should_build_an_enumerable_if_no_nodes_are_added()
        {
            var factory = new ObjectFactory<Person>();

            IEnumerable<Person> persons = factory.Build();

            persons.Should().NotBeNull();
        }

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

        public class FuncTransformTests
        {
            [Fact]
            public void It_should_apply_the_transform()
            {
                var funcTransform = new FuncTransform<Person, string>(x => x.FirstName = "Maria");
                var person = new Person();
                
                funcTransform.ApplyTo(person);

                person.FirstName.Should().Be("Maria");
            }
        }

        public class GuardedTransformsTests
        {
            [Fact]
            public void It_should_apply_if_no_conditions_were_set()
            {
                var guardSpecification = new AlwaysApplyTransformGuardSpecification();
                
                var canApply = guardSpecification.CanApply(currentIndex: 0);
                
                canApply.Should().BeTrue();
            }
            
            [Theory]
            [InlineData(0, 2, true)]
            [InlineData(1, 2, true)]
            [InlineData(2, 2, false)]
            public void It_should_apply_only_to_the_first_n_instances(int currentIndex, int countToApply, bool expected)
            {
                var guardSpecification = new ApplyTransformToFirstInstancesSpecification(countToApply);
                
                var canApply = guardSpecification.CanApply(currentIndex);
                
                canApply.Should().Be(expected);
            }
            
            [Theory]
            [InlineData(0, 2, 4, false)]
            [InlineData(1, 2, 4, false)]
            [InlineData(2, 2, 4, true)]
            [InlineData(3, 2, 4, true)]
            public void It_should_apply_only_to_the_last_n_instances(int currentIndex, int countToApply, int targetCount, bool expected)
            {
                var guardSpecification = new ApplyTransformToLastInstancesSpecification(countToApply, targetCount);
                
                var canApply = guardSpecification.CanApply(currentIndex);
                
                canApply.Should().Be(expected);
            }
        }

        [Fact]
        public void It_should_build_upon_the_added_generator_nodes()
        {
            var factory = new ObjectFactory<Person>();

            factory.AddNode(new GeneratorNode<Person>(targetCount: 1));
            factory.AddNode(new GeneratorNode<Person>(targetCount: 2));
            var persons = factory.Build();

            persons.Should().HaveCount(3);
        }

        [Fact]
        public void It_should_apply_default_transforms_to_all_generator_nodes()
        {
            var factory = new ObjectFactory<Person>();
            factory.AddNode(new GeneratorNode<Person>(targetCount: 1));
            factory.AddNode(new GeneratorNode<Person>(targetCount: 2));
            factory.AddDefaultTransform(new FuncTransform<Person, string>(x => x.FirstName = "Clark"));

            var persons = factory.Build();

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Clark");
            }
        }
    }

    internal class DummyTransform<T> : Transform<T>
        where T : class
    {
        public override void ApplyTo(T instance)
        {
        }
    }
}