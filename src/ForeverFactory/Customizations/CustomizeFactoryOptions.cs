using System;
using System.Collections.Generic;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Customizations
{
    internal class OptionsCollector<T> : IOptionsCollector<T>
        where T : class
    {
        private readonly Action<ICustomizeFactoryOptions<T>> _customize;
        private readonly ObjectFactoryOptions<T> _options;

        public OptionsCollector(Action<ICustomizeFactoryOptions<T>> customize)
        {
            _customize = customize;
            _options = new ObjectFactoryOptions<T>();
        }

        public IObjectFactoryOptions<T> Collect()
        {
            var customizationOptions = new CustomizeFactoryOptions2<T>(_options);
            _customize.Invoke(customizationOptions);

            return _options;
        }
        
        internal void UpdateConstructor(Func<T> customConstructor)
        {
            _options.CustomConstructor = customConstructor;
        }
        
        internal void UpdateBehavior(Behavior behavior)
        {
            _options.SelectedBehavior = behavior;
        }
    }

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

    internal class CustomizeFactoryOptions2<T> : ICustomizeFactoryOptions<T>
        where T : class
    {
        private readonly ObjectFactoryOptions<T> _objectFactoryOptions;

        public CustomizeFactoryOptions2(ObjectFactoryOptions<T> objectFactoryOptions)
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