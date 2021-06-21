using System;
using ForeverFactory.Behaviors;

namespace ForeverFactory.Customizations
{
    public class CustomizeFactoryOptions<T> : ICustomizeFactoryOptions<T>
        where T : class
    {
        private readonly MagicFactory<T> _magicFactory;

        public CustomizeFactoryOptions(MagicFactory<T> magicFactory)
        {
            _magicFactory = magicFactory;
        }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            _magicFactory.UsingConstructor(customConstructor);
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            _magicFactory.WithDefault(setMember);
            return this;
        }

        public ICustomizeFactoryOptions<T> SetDefaultBehavior(Behavior behavior)
        {
            _magicFactory.WithBehavior(behavior);
            return this;
        }
    }
}