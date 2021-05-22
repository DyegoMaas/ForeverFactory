using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class OneBuilder<T> : IOneBuilder<T>
        where T : class
    {
        private readonly List<Transform<T>>  _defaultTransforms = new List<Transform<T>>();
        private readonly List<Transform<T>> _transforms = new List<Transform<T>>();
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

        public void SetDefault<TValue>(Func<T,TValue> setMember)
        {
            _defaultTransforms.Add(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
        }

        public T Build()
        {
            var instance = _customConstructor?.Invoke() ?? Activator.CreateInstance<T>();
            foreach (var transform in _defaultTransforms.Union(_transforms))
            {
                transform.ApplyTo(instance);
            }
            
            return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(_defaultTransforms, this);
        }
    }
}