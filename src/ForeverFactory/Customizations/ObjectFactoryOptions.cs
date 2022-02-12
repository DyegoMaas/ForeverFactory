using System;
using System.Collections.Generic;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Customizations
{
    internal class ObjectFactoryOptions<T> : IObjectFactoryOptions<T>
        where T : class
    {
        public Func<T> CustomConstructor { get; internal set; }
        public Behavior SelectedBehavior { get; internal set; }
        public IList<Transform<T>> Transforms { get; }

        public ObjectFactoryOptions()
        {
            Transforms = new List<Transform<T>>();
            SelectedBehavior = new DoNotFillBehavior();
        }
    }
}