using System;
using System.Collections.Generic;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Generators.Options
{
    internal interface IObjectFactoryOptions<T>
        where T : class
    {
        Func<T> CustomConstructor { get; }
        IList<Transform<T>> Transforms { get; }
        Behavior SelectedBehavior { get; }
    }
}