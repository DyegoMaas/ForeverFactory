using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class FactoryTests
    {
        public class OneInstanceFactoryTests
        {
            private readonly FixedPersonFactory _factory;

            public OneInstanceFactoryTests()
            {
                _factory = new FixedPersonFactory();
            }
            
            [Fact]
            public void A_custom_factory_should_produce_objects_with_values_configured_in_its_constructor()
            {
                var person = _factory.Build();

                person.FirstName.Should().Be("Albert");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(56);
            }

            [Fact]
            public void Individual_properties_can_be_overwritten()
            {
                var person = _factory
                    .With(x => x.FirstName = "Guilherme")
                    .With(x => x.Age = 19)
                    .Build();

                person.FirstName.Should().Be("Guilherme");
                person.LastName.Should().Be("Einstein");
                person.Age.Should().Be(19);
            }
        }

        public class ManyInstanceBuilderTests
        {
            private readonly FixedPersonFactory _factory;

            public ManyInstanceBuilderTests()
            {
                _factory = new FixedPersonFactory();
            }
            
            [Fact]
            public void It_produces_many_instances_with_default_configuration()
            {
                var persons = _factory
                    .Many(count: 10)
                    .Build()
                    .ToList();

                persons.Should().HaveCount(10);
                foreach (var person in persons)
                {
                    person.FirstName.Should().Be("Albert");
                    person.LastName.Should().Be("Einstein");
                    person.Age.Should().Be(56);
                }
            }
            
            [Fact]
            public void It_produces_many_instances_with_overwritten_properties()
            {
                var persons = _factory
                    .Many(count: 10)
                    .With(x => x.Age = 19)
                    .With(x => x.LastName = "Nobel")
                    .Build()
                    .ToList();

                persons.Should().HaveCount(10);
                foreach (var person in persons)
                {
                    person.FirstName.Should().Be("Albert");
                    person.LastName.Should().Be("Nobel");
                    person.Age.Should().Be(19);
                }
            }
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class FixedPersonFactory : MagicFactory<Person>
    {
        public FixedPersonFactory()
        {
            Set(x => x.FirstName = "Albert");
            Set(x => x.LastName = "Einstein");
            Set(x => x.Age = 56);
        }
    }
    
    public abstract class Transform<T>
    {
        public abstract void ApplyTo(T instance);
    }

    public sealed class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly Func<T, TValue> _setMember;

        public FuncTransform(Func<T, TValue> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember(instance);
        }
    }

    public class MagicFactory<T> : IOneBuilder<T>
        where T : new() // TODO see if it possible to allow constructors with parameters
    {
        private readonly OneBuilder<T> _oneBuilder = new();
        private readonly List<Transform<T>> _defaultTransforms = new();
        
        protected void Set<TValue>(Expression<Func<T, TValue>> member, Func<T, TValue> value)
        {
            throw new NotImplementedException();
        }

        protected void Set<TValue>(Func<T, TValue> setMember)
        {
            _defaultTransforms.Add(new FuncTransform<T,TValue>(setMember));
            _oneBuilder.With(setMember); // TODO refactor in order to not repeat this operation
        }
        
        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _oneBuilder.With(setMember);
            return _oneBuilder;
        }

        public IManyBuilder<T> Many(int count)
        {
            return new ManyBuilder<T>(count, _defaultTransforms);
        }

        public T Build()
        {
            return _oneBuilder.Build();
        }
    }

    public class OneBuilder<T> : IOneBuilder<T>
        where T : new()
    {
        private readonly List<Transform<T>> _transforms = new();

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember));
            return this;
        }

        public T Build()
        {
            T instance = new();
            foreach (var transform in _transforms)
            {
                transform.ApplyTo(instance);
            }
            
            return instance;
        }
    }

    public class ManyBuilder<T> : IManyBuilder<T>
        where T : new()
    {
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _count;

        public ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms)
        {
            _count = count;
            _transforms.AddRange(defaultTransforms);
        }

        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T,TValue>(setMember));
            return this;
        }

        IEnumerable<T> IManyBuilder<T>.Build()
        {
            for (var i = 0; i < _count; i++)
            {
                T instance = new();
                foreach (var transform in _transforms)
                {
                    transform.ApplyTo(instance);
                }
            
                yield return instance;
            }
        }
    }

    public interface IOneBuilder<out T>
    {
        IOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
        T Build();
    }
    
    public interface IManyBuilder<out T>
    {
        IManyBuilder<T> With<TValue>(Func<T, TValue> setMember);
        IEnumerable<T> Build();
    }
}