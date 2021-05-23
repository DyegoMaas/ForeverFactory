using System;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders.Common
{
    internal interface ISharedContext<T>
        where T : class
    {
        TransformList<T> DefaultTransforms { get; }
        Func<T> CustomConstructor { get; }
    }
}