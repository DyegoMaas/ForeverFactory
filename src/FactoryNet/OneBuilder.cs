using System;
using System.Collections.Generic;

namespace FactoryNet
{
    public class OneBuilder<T> : IOneBuilder<T>
        where T : new()
    {
        private readonly List<Transform<T>> _transforms = new();

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
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
}