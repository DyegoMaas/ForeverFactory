using System;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders.Common
{
    internal class SharedContext<T> : ISharedContext<T>
        where T : class
    {
        public TransformList<T> DefaultTransforms { get; }
        public Func<T> CustomConstructor { get; }

        public SharedContext(TransformList<T> defaultTransforms, Func<T> customConstructor)
        {
            DefaultTransforms = defaultTransforms;
            CustomConstructor = customConstructor;
        }
    }
}