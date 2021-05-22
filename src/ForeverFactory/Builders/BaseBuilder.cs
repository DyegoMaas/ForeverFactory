using System.Collections.Generic;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders
{
    internal abstract class BaseBuilder<T>
        where T : class
    {
        protected TransformList<T> DefaultTransforms { get; } = new TransformList<T>();
        private TransformList<T> Transforms { get; } = new TransformList<T>(); // TODO rename to ConfigurationTransforms?
  
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
}