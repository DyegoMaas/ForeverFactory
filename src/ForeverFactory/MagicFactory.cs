using System;
using System.Collections.Generic;
using ForeverFactory.Builders;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory
{
    public static class MagicFactory
    {
        public static MagicFactory<T> For<T>() where T : class => new DefaultFactory<T>();
    }

    public abstract class MagicFactory<T> : IOneBuilder<T>
        where T : class
    {
        private readonly OneBuilder<T> _oneBuilder = new OneBuilder<T>();
        private readonly List<Transform<T>> _defaultTransforms = new List<Transform<T>>();
        private Func<T> _customConstructor;

        protected void UseConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _oneBuilder.SetCustomConstructor(customConstructor);
        }

        public MagicFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            UseConstructor(customConstructor);
            return this;
        }

        protected void Set<TValue>(Func<T, TValue> setMember)
        {
            _defaultTransforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            _oneBuilder.With(setMember); // TODO refactor in order to not repeat this operation
        }

        # region OneBuilder Wrapper
        
        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _oneBuilder.With(setMember);
            return _oneBuilder;
        }

        public T Build()
        {
            return _oneBuilder.Build();
        }
        
        #endregion

        public IManyBuilder<T> Many(int count)
        {
            return new ManyBuilder<T>(count, _defaultTransforms, _customConstructor);
        }
    }
}