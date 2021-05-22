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

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, _defaultTransforms, customConstructor: null, previousBuilder: new OneBuilderToLinkedOneBuilderAdapter<T>(this)); // TODO review custom constructor (difference between one set in custom factory and with UsingConstructor 
        }

        private class OneBuilderToLinkedOneBuilderAdapter<T> : ILinkedBuilder<T> where T : class
        {
            private readonly OneBuilder<T> _builder;

            public OneBuilderToLinkedOneBuilderAdapter(OneBuilder<T> builder)
            {
                _builder = builder;
            }

            public IEnumerable<T> Build()
            {
                yield return _builder.Build();
            }
        }
    }
}