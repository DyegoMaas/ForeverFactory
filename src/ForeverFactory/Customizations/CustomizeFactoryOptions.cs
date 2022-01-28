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
        // private readonly MagicFactory<T> _magicFactory;
        //
        public CustomizeFactoryOptions()
        {
            // _magicFactory = magicFactory;
            Transforms = new List<Transform<T>>();
            SelectedBehavior = new DoNotFillBehavior();
        }

        public Func<T> CustomConstructor { get; private set; }
        public IList<Transform<T>> Transforms { get; }
        public Behavior SelectedBehavior { get; private set; }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            // _magicFactory.UsingConstructor(customConstructor);
            CustomConstructor = customConstructor;
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            // _magicFactory.AddDefaultTransform(setMember);
            Transforms.Add(new FuncTransform<T, TValue>(setMember.Invoke));
            return this;
        }

        public ICustomizeFactoryOptions<T> SetDefaultBehavior(Behavior behavior)
        {
            // _magicFactory.WithBehavior(behavior);
            SelectedBehavior = behavior;
            return this;
        }

        public void UpdateConstructor(Func<T> customConstructor)
        {
            CustomConstructor = customConstructor;
        }

        public void UpdateBehavior(Behavior behavior)
        {
            SelectedBehavior = behavior;
        }
    }
}