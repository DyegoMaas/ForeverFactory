using System;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Customizations
{
    internal class CustomizeFactoryOptions<T> : ICustomizeFactoryOptions<T>
        where T : class
    {
        private readonly ObjectFactoryOptions<T> _objectFactoryOptions;

        public CustomizeFactoryOptions(ObjectFactoryOptions<T> objectFactoryOptions)
        {
            _objectFactoryOptions = objectFactoryOptions;
        }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            _objectFactoryOptions.CustomConstructor = customConstructor;
            return this;
        }

        public ICustomizeFactoryOptions<T> SetDefaultBehavior(Behavior behavior)
        {
            _objectFactoryOptions.SelectedBehavior = behavior;
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            _objectFactoryOptions.Transforms.Add(new FuncTransform<T, TValue>(setMember.Invoke));
            return this;
        }
    }
}