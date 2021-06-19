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
        public void It_should_be_easy_to_share_context_between_links()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public void It_should_be_easy_to_test()
        {
            throw new NotImplementedException();
        }

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
                    guard: new NoGuardSpecification()
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
                    guard: new ApplyTransformToFirstInstances(countToApply: 2)
                );
                generatorNode.AddTransform(
                    transform: new FuncTransform<Person, string>(x => x.FirstName = "Mirage"),
                    guard: new ApplyTransformToLastInstances(countToApply: 2, targetCount: 5)
                );
                
                var persons = generatorNode.ProduceInstances().ToArray();

                var firstNames = persons.Select(x => x.FirstName);
                firstNames.Should().BeEquivalentTo("Martha", "Martha", "Anne", "Mirage", "Mirage");
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
                var guardSpecification = new NoGuardSpecification();
                
                var canApply = guardSpecification.CanApply(currentIndex: 0);
                
                canApply.Should().BeTrue();
            }
            
            [Theory]
            [InlineData(0, 2, true)]
            [InlineData(1, 2, true)]
            [InlineData(2, 2, false)]
            public void It_should_apply_only_to_the_first_n_instances(int currentIndex, int countToApply, bool expected)
            {
                var guardSpecification = new ApplyTransformToFirstInstances(countToApply);
                
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
                var guardSpecification = new ApplyTransformToLastInstances(countToApply, targetCount);
                
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

        // [Fact]
        // public void It_should_add_new_generator_nodes()
        // {
        //     var factory = new ObjectFactory<Person>();
        //
        //     factory.AddNode(new GeneratorNode<Person>());
        // }

        [Fact]
        public void It_should_accept_transformations()
        {
            var factory = new ObjectFactory<Person>();

            factory.AddDefaultTransform(new DummyTransform<Person>());

            IEnumerable<Person> persons = factory.Build();
            throw new NotImplementedException("first, add nodes");
        }
    }

    internal abstract class TransformGuardSpecification
    {
        public abstract bool CanApply(int currentIndex);
    }
    
    internal class NoGuardSpecification : TransformGuardSpecification
    {
        public override bool CanApply(int currentIndex) => true;
    }

    internal class ApplyTransformToFirstInstances : TransformGuardSpecification
    {
        private readonly int _countToApply;

        public ApplyTransformToFirstInstances(int countToApply)
        {
            _countToApply = countToApply;
        }
        
        public override bool CanApply(int currentIndex)
        {
            return currentIndex < _countToApply;
        }
    }

    internal class GuardedTransform<T> 
        where T : class
    {
        public Transform<T> Transform { get; }
        public TransformGuardSpecification Guard { get; }

        public GuardedTransform(Transform<T> transform, TransformGuardSpecification guard)
        {
            Transform = transform;
            Guard = guard;
        }
    }

    internal class NotGuardedTransform<T> : GuardedTransform<T>
        where T : class
    {
        public NotGuardedTransform(Transform<T> transform) 
            : base(transform, new NoGuardSpecification())
        {
        }
    }
    
    internal class ApplyTransformToLastInstances : TransformGuardSpecification
    {
        private readonly int _countToApply;
        private readonly int _targetCount;

        public ApplyTransformToLastInstances(int countToApply, int targetCount)
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

        public void AddTransform(Transform<T> transform, TransformGuardSpecification guard = null)
        {
            guard ??= new NoGuardSpecification();
            _transformsToApply.Add(new GuardedTransform<T>(transform, guard));
        }

        public IEnumerable<T> ProduceInstances()
        {
            for (var index = 0; index < _targetCount; index++)
            {
                var instance = CreateInstance();
                ApplyTranforms(instance, index);
                
                yield return instance;
            }
        }

        private T CreateInstance()
        {
            return _customConstructor != null 
                ? _customConstructor.Invoke() 
                : Activator.CreateInstance<T>();
        }

        private void ApplyTranforms(T instance, int index)
        {
            foreach (var guardedTransform in _transformsToApply)
            {
                if (guardedTransform.Guard.CanApply(index))
                {
                    guardedTransform.Transform.ApplyTo(instance);
                }
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

    internal class ObjectFactory<T> : IBuilder<T> 
        where T : class
    {
        private readonly LinkedList<GeneratorNode<T>> _nodes = new LinkedList<GeneratorNode<T>>();

        public void AddDefaultTransform(Transform<T> transform)
        {
            throw new NotImplementedException();
        }

        public void AddNode(GeneratorNode<T> generatorNode)
        {
            _nodes.AddLast(generatorNode);
        }

        public IEnumerable<T> Build()
        {
            return _nodes.SelectMany(generatorNode => generatorNode.ProduceInstances());
        }
    }
    
    
}