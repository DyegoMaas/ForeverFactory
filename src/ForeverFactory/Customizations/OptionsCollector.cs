using System;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators.Options;

namespace ForeverFactory.Customizations
{
    internal class OptionsCollector<T> : IOptionsCollector<T>
        where T : class
    {
        private readonly Action<ICustomizeFactoryOptions<T>> _customize;
        private readonly ObjectFactoryOptions<T> _options;
        private readonly CustomizeFactoryOptions<T> _customizationOptions;

        public OptionsCollector(Action<ICustomizeFactoryOptions<T>> customize)
        {
            _customize = customize;
            
            _options = new ObjectFactoryOptions<T>();
            _customizationOptions = new CustomizeFactoryOptions<T>(_options);
            LoadStaticCustomizationInto(_customizationOptions);
        }

        public IObjectFactoryOptions<T> Collect()
        {
            _customize.Invoke(_customizationOptions);

            return _options;
        }

        private static void LoadStaticCustomizationInto(ICustomizeFactoryOptions<T> customizationOptions)
        {
            if (ForeverFactoryGlobalSettings.GlobalBehavior != null)
            {
                customizationOptions.SetDefaultBehavior(ForeverFactoryGlobalSettings.GlobalBehavior);
            }
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