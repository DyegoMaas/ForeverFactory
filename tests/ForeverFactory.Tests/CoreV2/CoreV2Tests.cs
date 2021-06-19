using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Builders;
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

    internal abstract class ApplyTransformGuardSpecification
    {
        public abstract bool CanApply(int currentIndex);
    }
    
    internal class AlwaysApplyTransformGuardSpecification : ApplyTransformGuardSpecification
    {
        public override bool CanApply(int currentIndex) => true;
    }

    internal class ApplyTransformToFirstInstancesSpecification : ApplyTransformGuardSpecification
    {
        private readonly int _countToApply;

        public ApplyTransformToFirstInstancesSpecification(int countToApply)
        {
            _countToApply = countToApply;
        }
        
        public override bool CanApply(int currentIndex)
        {
            return currentIndex < _countToApply;
        }
    }

    internal class ApplyTransformToLastInstancesSpecification : ApplyTransformGuardSpecification
    {
        private readonly int _countToApply;
        private readonly int _targetCount;

        public ApplyTransformToLastInstancesSpecification(int countToApply, int targetCount)
        {
            _countToApply = countToApply;
            _targetCount = targetCount;
        }
        
        public override bool CanApply(int currentIndex)
        {
            var firstToApply = _targetCount - _countToApply;
            return currentIndex >= firstToApply;
        }
    }

    internal record GuardedTransform<T> 
        where T : class
    {
        public Transform<T> Transform { get; }
        public ApplyTransformGuardSpecification Guard { get; }

        public GuardedTransform(Transform<T> transform, ApplyTransformGuardSpecification guard)
        {
            Transform = transform;
            Guard = guard;
        }
    }

    internal record NotGuardedTransform<T> : GuardedTransform<T>
        where T : class
    {
        public NotGuardedTransform(Transform<T> transform) 
            : base(transform, new AlwaysApplyTransformGuardSpecification())
        {
        }
    }

    public abstract class Transform<T>
    {
        public abstract void ApplyTo(T instance);
    }

    internal delegate TAffectedProperty SetMember<in T, out TAffectedProperty>(T arg);

    internal class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly SetMember<T, TValue> _setMember;

        public FuncTransform(SetMember<T, TValue> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember.Invoke(instance);
        }
    }
            
    internal class GeneratorNode<T>
        where T : class
    {
        private readonly int _targetCount;
        private readonly Func<T> _customConstructor = null;
        private readonly List<GuardedTransform<T>> _transformsToApply = new List<GuardedTransform<T>>();

        public GeneratorNode(
            int targetCount = 1, 
            Func<T> customConstructor = null)
        {
            _targetCount = targetCount;
            _customConstructor = customConstructor;
        }

        public void AddTransform(Transform<T> transform, ApplyTransformGuardSpecification guard = null)
        {
            guard ??= new AlwaysApplyTransformGuardSpecification();
            _transformsToApply.Add(new GuardedTransform<T>(transform, guard));
        }

        public IEnumerable<T> ProduceInstances(IEnumerable<NotGuardedTransform<T>> defaultTransforms = null)
        {
            for (var index = 0; index < _targetCount; index++)
            {
                var instance = CreateInstance();

                var defaultTransformsToApply = defaultTransforms ?? Enumerable.Empty<GuardedTransform<T>>();
                var transformsToApply = defaultTransformsToApply.Union(_transformsToApply);
                ApplyTransformsToInstance(transformsToApply, instance, index);
                
                yield return instance;
            }
        }

        private T CreateInstance()
        {
            return _customConstructor != null 
                ? _customConstructor.Invoke() 
                : Activator.CreateInstance<T>();
        }

        private void ApplyTransformsToInstance(IEnumerable<GuardedTransform<T>> guardedTransforms, T instance, int instanceIndex)
        {
            foreach (var guardedTransform in guardedTransforms)
            {
                if (guardedTransform.Guard.CanApply(instanceIndex))
                {
                    guardedTransform.Transform.ApplyTo(instance);
                }
            }
        }
    }

    internal class ObjectFactory<T> : IBuilder<T> 
        where T : class
    {
        private readonly LinkedList<GeneratorNode<T>> _nodes = new LinkedList<GeneratorNode<T>>();
        private readonly List<NotGuardedTransform<T>> _defaultTransforms = new List<NotGuardedTransform<T>>();
        
        public void AddDefaultTransform(Transform<T> transform)
        {
            _defaultTransforms.Add(new NotGuardedTransform<T>(transform));
        }

        public void AddNode(GeneratorNode<T> generatorNode)
        {
            _nodes.AddLast(generatorNode);
        }

        public IEnumerable<T> Build()
        {
            return _nodes.SelectMany(generatorNode => generatorNode.ProduceInstances(_defaultTransforms));
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