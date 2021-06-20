using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Builders;
using ForeverFactory.Core;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards.Specifications;

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

        protected MagicFactory()
        {
            SetRootNode(targetCount: 1);
        }

        private void SetRootNode(int targetCount)
        {
            _rootNode = new GeneratorNode<T>(targetCount, _customConstructor);
            _objectFactory.AddRootNode(_rootNode);
        }

        protected void UseConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _rootNode.OverrideCustomConstructor(newCustomConstructor: customConstructor);
        }

        public ICustomizableFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            UseConstructor(customConstructor);
            return this;
        }

        protected void Set<TValue>(Func<T, TValue> setMember)
        {
            _objectFactory.AddDefaultTransform(new FuncTransform<T,TValue>(setMember.Invoke));
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

        private void AddTransformThatAlwaysApply<TValue>(Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: new AlwaysApplyTransformSpecification()
            );
        }

        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T,TValue>(setMember.Invoke),
                guard: new ApplyTransformToFirstInstancesSpecification(count)
            );
            return this;
        }

        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T,TValue>(setMember.Invoke),
                guard: new ApplyTransformToLastInstancesSpecification(count, node.TargetCount)
            );
            return this;
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