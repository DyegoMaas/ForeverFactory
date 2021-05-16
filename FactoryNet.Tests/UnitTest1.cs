using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace FactoryNet.Tests
{
    public class UnitTest1
    {
        private readonly FixedPersonFactory _factory;

        public UnitTest1()
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

        [Fact]
        public void Test2()
        {
            IEnumerable<Person> persons = _factory.Many(count: 10).Build();
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
            Config(x => x.FirstName = "Albert");
            Config(x => x.LastName = "Einstein");
            Config(x => x.Age = 56);
        }
    }

    public interface INamesGenerator
    {
        string TakeOne();
    }

    public interface IManyFactory
    {
        IEnumerable<Person> Build();
    }

    public class MagicFactory<T> : IOneBuilder<T>
        where T : new() // TODO see if it possible to allow constructors with parameters
    {
        private abstract class Transform<T>
        {
            public abstract void ApplyTo(T instance);
        }

        private sealed class FuncTransform<T, TValue> : Transform<T>
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
    // protected void Config(Expression<Func<T, object>> member, string value)
        // {
        //     throw new NotImplementedException();
        // }

        private readonly List<Transform<T>> _transforms = new();
        
        protected void Config<TValue>(Expression<Func<T, TValue>> member, Func<T, TValue> value)
        {
            throw new NotImplementedException();
        }

        protected void Config<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember));
        }
        
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

        public IManyFactory Many(int count)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IOneBuilder<T> : IBuilder<T>
    {
        IOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
    }

    public interface IBuilder<T>
    {
        T Build();
    }
}