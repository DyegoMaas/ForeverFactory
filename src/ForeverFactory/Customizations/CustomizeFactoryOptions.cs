using System;
using System.Collections.Generic;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Customizations
{
    internal class CustomizeFactoryOptions<T> : ICustomizeFactoryOptions<T>, IObjectFactoryOptions<T>, IOptionsCollector<T>
        where T : class
    {
        private readonly Action<ICustomizeFactoryOptions<T>> _initialize;

        public CustomizeFactoryOptions(Action<ICustomizeFactoryOptions<T>> initialize)
        {
            _initialize = initialize;
            
            Transforms = new List<Transform<T>>();
            SelectedBehavior = new DoNotFillBehavior();
        }

        public Func<T> CustomConstructor { get; private set; }
        public Behavior SelectedBehavior { get; private set; }

        public IList<Transform<T>> Transforms { get; }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            CustomConstructor = customConstructor;
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            Transforms.Add(new FuncTransform<T, TValue>(setMember.Invoke));
            return this;
        }

        public ICustomizeFactoryOptions<T> SetDefaultBehavior(Behavior behavior)
        {
            SelectedBehavior = behavior;
            return this;
        }

        internal void UpdateConstructor(Func<T> customConstructor)
        {
            CustomConstructor = customConstructor;
        }

        internal void UpdateBehavior(Behavior behavior)
        {
            SelectedBehavior = behavior;
        }

        IObjectFactoryOptions<T> IOptionsCollector<T>.Initialize()
        {
            _initialize.Invoke(this);
            return this;
        }
    }
}