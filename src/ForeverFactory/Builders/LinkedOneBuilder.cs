using System;
using System.Collections.Generic;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class LinkedOneBuilder<T> : BaseBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly IOneBuilder<T> _previousNotLinked;
        private readonly ILinkedBuilder<T> _previousLinked;
        private Func<T> _customConstructor;

        public LinkedOneBuilder(TransformList<T> defaultTransforms, ILinkedBuilder<T> previousLinked)
        {
            AddDefaultTransforms(defaultTransforms);
            _previousLinked = previousLinked;
        }
        
        public LinkedOneBuilder(TransformList<T> defaultTransforms, IOneBuilder<T> previousNotLinked)
        {
            AddDefaultTransforms(defaultTransforms);
            _previousNotLinked = previousNotLinked;
        }
        
        public void SetCustomConstructor(Func<T> customConstructor) // TODO add tests
        {
            _customConstructor = customConstructor;
        }

        public ILinkedOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        public IEnumerable<T> Build()
        {
            if (_previousNotLinked != null)
                yield return _previousNotLinked.Build();
            
            if (_previousLinked != null)
            {
                foreach (var linkedInstance in _previousLinked.Build())
                {
                    yield return linkedInstance;
                }
            }

            var instance = _customConstructor?.Invoke() ?? Activator.CreateInstance<T>();
            foreach (var transform in GetTransformsToApply())
            {
                transform.ApplyTo(instance);
            }
            
            yield return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(DefaultTransforms, this);
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, DefaultTransforms, _customConstructor, this);
        }
    }
}