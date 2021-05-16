using System;
using System.Collections.Generic;
using FactoryNet.Transforms;
using FactoryNet.Transforms.Conditions;

namespace FactoryNet.Builders
{
    internal class OneBuilder<T> : IOneBuilder<T>
        where T : class
    {
        private readonly List<Transform<T>> _transforms = new();
        private Func<T> _customConstructor;

        public void SetCustomConstructor(Func<T> customConstructor) 
        {
            _customConstructor = customConstructor;
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        public T Build()
        {
            var instance = _customConstructor?.Invoke() ?? Activator.CreateInstance<T>();
            foreach (var transform in _transforms)
            {
                transform.ApplyTo(instance);
            }
            
            return instance;
        }
    }
}