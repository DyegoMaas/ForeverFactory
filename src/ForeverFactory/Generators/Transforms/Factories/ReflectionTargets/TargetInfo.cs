using System;
using System.Diagnostics;

namespace ForeverFactory.Generators.Transforms.Factories.ReflectionTargets
{
    [DebuggerDisplay("{TargetType}: {Name}")]
    internal abstract class TargetInfo
    {
        public abstract Type TargetType { get; }
        public abstract string Name { get; }
            
        public abstract void SetValue(object instance, object value);
    };
}