using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Builders;
using ForeverFactory.Core;
using ForeverFactory.Core.FactoryBehaviors;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using ForeverFactory.Core.Transforms.Specialized;
using ForeverFactory.Customizations;

namespace ForeverFactory
{
    /// <summary>
    /// Allows the creation of a customizable factory.
    /// </summary>
    public static class MagicFactory
    {
        /// <summary>
        /// Creates a factory for building instances of type "T".
        /// </summary>
        /// <typeparam name="T">The factory will build instances of this type</typeparam>
        /// <returns>Factory of T</returns>
        public static ICustomizableFactory<T> For<T>() where T : class => new DynamicFactory<T>();
        
        private sealed class DynamicFactory<T> : MagicFactory<T>
            where T : class
        {
            protected override void Customize(ICustomizeFactoryOptions<T> customization)
            {
            }
        }
    }
    
    /// <summary>
    /// A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory<T> : ICustomizableFactory<T>, IManyBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly ObjectFactory<T> _objectFactory = new ObjectFactory<T>();
        private Func<T> _customConstructor;
        private GeneratorNode<T> _rootNode;
        private Behaviors _chosenBehavior = Behaviors.DotNotFill;

        protected MagicFactory()
        {
            SetRootNode(targetCount: 1);
            Customize(new CustomizeFactoryOptions<T>(this));
        }
        
        // TODO add test to ensure default transforms are not lost in the process
        private void SetRootNode(int targetCount)
        {
            _rootNode = new GeneratorNode<T>(targetCount, _customConstructor);
            _objectFactory.AddRootNode(_rootNode);
        }
        
        protected abstract void Customize(ICustomizeFactoryOptions<T> customization);

        public ICustomizableFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _rootNode.OverrideCustomConstructor(newCustomConstructor: customConstructor);
            return this;
        }

        public ICustomizableFactory<T> WithBehavior(Behaviors chosenBehavior)
        {
            _chosenBehavior = chosenBehavior;

            switch (chosenBehavior)
            {
                case Behaviors.FillWithEmpty:
                    var transform = TransformFactory.GetFillWithEmptyFor<T>();
                    _objectFactory.AddDefaultTransform(transform);
                    break;
            }
            return this;
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        IManyBuilder<T> IManyBuilder<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        ILinkedOneBuilder<T> ILinkedOneBuilder<T>.With<TValue>(Func<T, TValue> setMember)
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

        private void AddTransformThatAlwaysApply<TValue>(Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: node => new AlwaysApplyTransformSpecification()
            );
        }

        private void AddTransformThatAppliesToFirstNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: node => new ApplyTransformToFirstInstancesSpecification(count)
            );
        }

        private void AddTransformThatAppliesToLastNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: node => new ApplyTransformToLastInstancesSpecification(count, node.TargetCount)
            );
        }

        public IManyBuilder<T> Many(int count)
        {
            SetRootNode(targetCount: count);
            return this;
        }
        
        public ILinkedOneBuilder<T> PlusOne()
        {
            var newNode = new GeneratorNode<T>(targetCount: 1, _customConstructor);
            _objectFactory.AddNode(newNode);
            
            return this;
        }

        public IManyBuilder<T> Plus(int count)
        {
            var newNode = new GeneratorNode<T>(targetCount: count, _customConstructor);
            _objectFactory.AddNode(newNode);

            return this;
        }
        
        public T Build()
        {
            return _objectFactory.Build().First();
        }

        IEnumerable<T> IBuilder<T>.Build()
        {
            return _objectFactory.Build();
        }
    }
}