using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Behaviors;
using ForeverFactory.Builders;
using ForeverFactory.Core;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using ForeverFactory.Customizations;

namespace ForeverFactory
{
    /// <summary>
    ///     Allows the creation of a customizable factory.
    /// </summary>
    public static class MagicFactory
    {
        /// <summary>
        ///     Creates a factory for building instances of type "T".
        /// </summary>
        /// <typeparam name="T">The factory will build instances of this type</typeparam>
        /// <returns>Factory of T</returns>
        public static ISimpleFactory<T> For<T>() where T : class
        {
            return new DynamicFactory<T>();
        }

        private sealed class DynamicFactory<T> : MagicFactory<T>
            where T : class
        {
            protected override void Customize(ICustomizeFactoryOptions<T> customization)
            {
            }
        }
    }

    /// <summary>
    ///     A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory<T> : ISimpleFactory<T>, IOneBuilder<T>, IManyBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly ObjectFactory<T> _objectFactory = new ObjectFactory<T>();
        private Func<T> _customConstructor;
        private GeneratorNode<T> _rootNode;

        protected MagicFactory()
        {
            SetRootNode(1);
            Customize(new CustomizeFactoryOptions<T>(this));
        }

        public ISimpleFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _rootNode.OverrideCustomConstructor(customConstructor);
            return this;
        }

        internal void AddDefaultTransform<TValue>(Func<T, TValue> setMember)
        {
            _objectFactory.AddDefaultTransform(new FuncTransform<T,TValue>(setMember.Invoke));
        }

        public ISimpleFactory<T> WithBehavior(Behavior behavior)
        {
            var transforms = behavior.GetTransforms<T>();
            foreach (var transform in transforms)
            {
                _objectFactory.AddDefaultTransform(transform);
            }
            return this;
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        public IManyBuilder<T> Many(int count)
        {
            SetRootNode(count);
            return this;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            var newNode = new GeneratorNode<T>(1, _customConstructor);
            _objectFactory.AddNode(newNode);
            return this;
        }

        public IManyBuilder<T> Plus(int count)
        {
            var newNode = new GeneratorNode<T>(count, _customConstructor);
            _objectFactory.AddNode(newNode);

            return this;
        }

        public T Build()
        {
            return _objectFactory.Build().First();
        }

        ILinkedOneBuilder<T> ILinkedOneBuilder<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        IManyBuilder<T> IManyBuilder<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToFirstNInstances(count, setMember);
            return this;
        }

        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToLastNInstances(count, setMember);
            return this;
        }

        IEnumerable<T> IEnumerableBuilder<T>.Build()
        {
            return _objectFactory.Build();
        }

        private void SetRootNode(int targetCount)
        {
            _rootNode = new GeneratorNode<T>(targetCount, _customConstructor);
            _objectFactory.AddRootNode(_rootNode);
        }

        protected abstract void Customize(ICustomizeFactoryOptions<T> customization);

        private void AddTransformThatAlwaysApply<TValue>(Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new AlwaysApplyTransformSpecification()
            );
        }

        private void AddTransformThatAppliesToFirstNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new ApplyTransformToFirstInstancesSpecification(count)
            );
        }

        private void AddTransformThatAppliesToLastNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new ApplyTransformToLastInstancesSpecification(count, node.TargetCount)
            );
        }
    }
}