using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal abstract class BaseBuilder<T>
        where T : class
    {
        protected TransformList<T> DefaultTransforms { get; } = new TransformList<T>();
        protected TransformList<T> Transforms { get; } = new TransformList<T>(); // TODO rename to ConfigurationTransforms?

        protected void AddDefaultTransforms(IEnumerable<Transform<T>> transforms)
        {
            DefaultTransforms.AddRange(transforms);
        }
        
        protected void AddDefaultTransforms(TransformList<T> transforms)
        {
            DefaultTransforms.AddRange(transforms);
        }
        
        protected void AddDefaultTransform(Transform<T> transform)
        {
            DefaultTransforms.Add(transform);
        }
        
        protected void AddTransform(Transform<T> transform)
        {
            Transforms.Add(transform);
        }

        protected IEnumerable<Transform<T>> GetTransformsToApply()
        {
            return DefaultTransforms
                .Union(Transforms)
                .AsEnumerable();
        }
    }

    internal class TransformList<T>
    {
        private readonly List<Transform<T>> _transforms = new List<Transform<T>>();

        public void AddRange(IEnumerable<Transform<T>> transforms)
        {
            _transforms.AddRange(transforms);
        }
        
        public void AddRange(TransformList<T> transformList)
        {
            _transforms.AddRange(transformList._transforms);
        }

        public void Add(Transform<T> transform)
        {
            _transforms.Add(transform);
        }

        public TransformList<T> Union(TransformList<T> otherList)
        {
            var newList = new TransformList<T>();
            newList.AddRange(_transforms);
            newList.AddRange(otherList._transforms);
            return newList;
        }

        public IEnumerable<Transform<T>> AsEnumerable()
        {
            return _transforms;
        }
    }

    internal class LinkedOneBuilder<T> : BaseBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly IOneBuilder<T> _previousNotLinked;
        // private readonly List<Transform<T>> _defaultTransforms = new List<Transform<T>>();
        private readonly ILinkedBuilder<T> _previousLinked;
        // private readonly List<Transform<T>> _transforms = new List<Transform<T>>();
        private Func<T> _customConstructor;

        public LinkedOneBuilder(TransformList<T> defaultTransforms, ILinkedBuilder<T> previousLinked)
        {
            AddDefaultTransforms(defaultTransforms);
            // DefaultTransforms.AddRange(defaultTransforms);
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
            // Transforms.Add(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
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