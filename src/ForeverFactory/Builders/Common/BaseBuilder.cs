using System;
using System.Collections.Generic;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders.Common
{
    internal abstract class BaseBuilder<T> 
        where T : class
    {
        public ISharedContext<T> SharedContext { get; }
        private TransformList<T> Transforms { get; }

        protected BaseBuilder(ISharedContext<T> sharedContext)
        {
            SharedContext = sharedContext;
            Transforms = new TransformList<T>();
        }
        
        protected void AddTransform(Transform<T> transform)
        {
            Transforms.Add(transform);
        }

        protected IEnumerable<Transform<T>> GetTransformsToApply()
        {
            return SharedContext.DefaultTransforms
                .Union(Transforms)
                .AsEnumerable();
        }

        protected T CreateInstance()
        {
            return SharedContext.CustomConstructor?.Invoke() ?? Activator.CreateInstance<T>();
        }
    }
}