using System;
using System.Collections.Generic;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Customizations
{
    public class CustomizeFactoryOptions<T> : ICustomizeFactoryOptions<T>, IObjectFactoryOptions<T>
        where T : class
    {
        private readonly List<Transform<T>> _transforms;
        
        public CustomizeFactoryOptions()
        {
            _transforms = new List<Transform<T>>();
            SelectedBehavior = new DoNotFillBehavior();
        }

        public Func<T> CustomConstructor { get; private set; }

        public IReadOnlyCollection<Transform<T>> Transforms => _transforms.AsReadOnly();

        public Behavior SelectedBehavior { get; private set; }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            CustomConstructor = customConstructor;
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T, TValue>(setMember.Invoke));
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
    }
}