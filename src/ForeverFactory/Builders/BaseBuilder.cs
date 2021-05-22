using System;
using System.Collections.Generic;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders
{
    internal abstract class BaseBuilder<T> : ISharedContext<T>
        where T : class
    {
        public TransformList<T> DefaultTransforms { get; }
        public  Func<T> CustomConstructor { get; private set; }
        private TransformList<T> Transforms { get; } // TODO rename to ConfigurationTransforms?

        protected BaseBuilder()
        {
            DefaultTransforms = new TransformList<T>();
            Transforms = new TransformList<T>();
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
        
        protected void SetCustomConstructor(Func<T> customConstructor) // TODO test scenarios where all the chain uses the same custom constructor
        {
            CustomConstructor = customConstructor;
        }

        protected IEnumerable<Transform<T>> GetTransformsToApply()
        {
            return DefaultTransforms
                .Union(Transforms)
                .AsEnumerable();
        }

        protected T CreateInstance()
        {
            return CustomConstructor?.Invoke() ?? Activator.CreateInstance<T>();
        }
    }
}