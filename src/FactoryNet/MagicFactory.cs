using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FactoryNet
{
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
}