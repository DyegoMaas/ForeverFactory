using System;

namespace ForeverFactory.Generators.Transforms.Factories.ReflectionTargets
{
    internal abstract class TargetInfo
    {
        public abstract Type TargetType { get; }
        public abstract string Name { get; }
            
        public abstract void SetValue(object instance, object value);
        
        public bool IsNullable() => GetNullableUnderlyingType() != null;
        public Type GetNullableUnderlyingType() => Nullable.GetUnderlyingType(TargetType);
    };
}