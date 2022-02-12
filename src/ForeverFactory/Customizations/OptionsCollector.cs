using System;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators;

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
            var customizationOptions = new CustomizeFactoryOptions<T>(_options);
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
}