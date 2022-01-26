using System;

namespace ForeverFactory.Generators.Transforms.Factories.ReflectionTargets
{
    internal abstract class TargetInfo
    {
        public abstract Type TargetType { get; }
        public abstract string Name { get; }
            
        public abstract void SetValue(object instance, object value);
    };
}