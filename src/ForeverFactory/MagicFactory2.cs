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
    public static class MagicFactory2
    {
        /// <summary>
        /// Creates a factory for building instances of type "T".
        /// </summary>
        /// <typeparam name="T">The factory will build instances of this type</typeparam>
        /// <returns>Factory of T</returns>
        public static ICustomizableFactory<T> For<T>() where T : class => new DynamicFactory<T>();
        
        private sealed class DynamicFactory<T> : MagicFactory2<T>
            where T : class
        {
        }
    }
    
    /// <summary>
    /// A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory2<T> : ICustomizableFactory<T>, IManyBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly ObjectFactory<T> _objectFactory = new ObjectFactory<T>();
        private Func<T> _customConstructor;
        private GeneratorNode<T> _rootNode;

        protected MagicFactory2()
        {
            SetRootNode(targetCount: 1);
        }

        private void SetRootNode(int targetCount)
        {
            _rootNode = new GeneratorNode<T>(targetCount: targetCount, customConstructor: null);
            _objectFactory.AddRootNode(_rootNode);
        }

        /// <summary>
        /// Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        protected void UseConstructor(Func<T> customConstructor)
        {
            _rootNode.CustomConstructor = customConstructor;
        }

        /// <summary>
        /// Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        public ICustomizableFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            UseConstructor(customConstructor);
            return this;
        }

        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
        protected void Set<TValue>(Func<T, TValue> setMember)
        {
            _objectFactory.AddDefaultTransform(new FuncTransform<T,TValue>(setMember.Invoke));
        }

        # region OneBuilder Wrapper
        
        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
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
                guard: new AlwaysApplyTransformGuardSpecification()
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

        /// <summary>
        /// Applies all configurations and builds a new object of type "T". 
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        public T Build()
        {
            return _objectFactory.Build().First();
        }

        /// <summary>
        /// Allows to build multiple objects. This method gives access to further group customization.
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        /// <returns>A builder for multiple objects.</returns>
        public IManyBuilder<T> Many(int count)
        {
            SetRootNode(targetCount: count);
            return this;
        }
        
        /// <summary>
        /// Creates a new builder of "T". It will build a new object, in addition to the previous configurations. 
        /// </summary>
        /// <returns>A builder of "T"</returns>
        public ILinkedOneBuilder<T> PlusOne()
        {
            var newNode = new GeneratorNode<T>(targetCount: 1, _customConstructor);
            _objectFactory.AddNode(newNode);
            
            return this;
        }

        /// <summary>
        /// Creates a new set of customizable objects, following the previous sets created used the "Many" or "Plus" methods.
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        public IManyBuilder<T> Plus(int count)
        {
            var newNode = new GeneratorNode<T>(targetCount: count, _customConstructor);
            _objectFactory.AddNode(newNode);

            return this;
        }

        private void AddRootNode()
        {
            var rootNode = new GeneratorNode<T>(targetCount: 1, _customConstructor);
            _objectFactory.AddNode(rootNode);
        }

        #endregion

        IEnumerable<T> IBuilder<T>.Build()
        {
            return _objectFactory.Build();
        }
    }
}