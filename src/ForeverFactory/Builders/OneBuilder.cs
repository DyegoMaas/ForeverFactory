using System;
using System.Collections.Generic;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class OneBuilder<T> : BaseBuilder<T>, IOneBuilder<T>
        where T : class
    {
        private Func<T> _customConstructor;

        public void SetCustomConstructor(Func<T> customConstructor) 
        {
            _customConstructor = customConstructor;
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        public void SetDefault<TValue>(Func<T,TValue> setMember)
        {
            AddDefaultTransform(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
        }

        public T Build()
        {
            var instance = _customConstructor?.Invoke() ?? Activator.CreateInstance<T>();
            foreach (var transform in GetTransformsToApply())
            {
                transform.ApplyTo(instance);
            }
            
            return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(DefaultTransforms, this);
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, DefaultTransforms, 
                customConstructor: null, 
                previousBuilder: new OneBuilderToLinkedOneBuilderAdapter(this))
            ; // TODO review custom constructor (difference between one set in custom factory and with UsingConstructor 
        }

        private class OneBuilderToLinkedOneBuilderAdapter : ILinkedBuilder<T>
        {
            private readonly OneBuilder<T> _builder;

            public OneBuilderToLinkedOneBuilderAdapter(OneBuilder<T> builder)
            {
                _builder = builder;
            }

            public ILinkedOneBuilder<T> PlusOne()
            {
                return new LinkedOneBuilder<T>(_builder.DefaultTransforms, _builder);
            }

            public IManyBuilder<T> Plus(int count)
            {
                return new ManyBuilder<T>(count, _builder.DefaultTransforms, 
                    customConstructor: null, 
                    previousBuilder: null
                );
            }

            public IEnumerable<T> Build()
            {
                yield return _builder.Build();
            }
        }
    }
}