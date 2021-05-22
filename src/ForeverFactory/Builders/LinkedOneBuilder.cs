using System;
using System.Collections.Generic;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class LinkedOneBuilder<T> : ILinkedOneBuilder<T>
        where T : class
    {
        private readonly IOneBuilder<T> _previousNotLinked;
        private readonly IEnumerable<Transform<T>> _defaultTransforms;
        private readonly ILinkedOneBuilder<T> _previousLinked;
        private readonly List<Transform<T>> _transforms = new List<Transform<T>>();
        private Func<T> _customConstructor;

        public LinkedOneBuilder(IEnumerable<Transform<T>> defaultTransforms, ILinkedOneBuilder<T> previousLinked)
        {
            _defaultTransforms = defaultTransforms;
            _transforms.AddRange(_defaultTransforms);
            _previousLinked = previousLinked;
        }
        
        public LinkedOneBuilder(IEnumerable<Transform<T>> defaultTransforms, IOneBuilder<T> previousNotLinked)
        {
            _defaultTransforms = defaultTransforms;
            _transforms.AddRange(_defaultTransforms);
            _previousNotLinked = previousNotLinked;
        }
        
        public void SetCustomConstructor(Func<T> customConstructor) 
        {
            _customConstructor = customConstructor;
        }

        public ILinkedOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
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
            foreach (var transform in _transforms)
            {
                transform.ApplyTo(instance);
            }
            
            yield return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(_defaultTransforms, this);
        }
    }
}