using System;
using System.Collections.Generic;
using FactoryNet.Builders;
using FactoryNet.Transforms;
using FactoryNet.Transforms.Conditions;

namespace FactoryNet
{
    public static class MagicFactory
    {
        public static MagicFactory<T> For<T>() where T : new() => new DefaultFactory<T>();
    }

    public abstract class MagicFactory<T> : IOneBuilder<T>
        where T : new() // TODO see if it possible to allow constructors with parameters
    {
        private readonly OneBuilder<T> _oneBuilder = new();
        private readonly List<Transform<T>> _defaultTransforms = new();
        
        protected void Set<TValue>(Func<T, TValue> setMember)
        {
            _defaultTransforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
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
}