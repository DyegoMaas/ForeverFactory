using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Behaviors;
using ForeverFactory.Customizations;
using ForeverFactory.FluentInterfaces;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Guards.Specifications;

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
    public abstract class MagicFactory<T> : ISimpleFactory<T>, ICustomizeOneBuildOneWithNavigation<T>, ICustomizeManyBuildMany<T>, ICustomizeOneBuildManyWithNavigation<T>
        where T : class
    {
        private readonly ObjectFactory<T> _objectFactory;
        // private Func<T> _customConstructor;

        private readonly CustomizeFactoryOptions<T> _customizeFactoryOptions;
        // private GeneratorNode<T> _rootNode;
        private GeneratorNode<T> _currentNode;

        protected MagicFactory()
        {
            // _rootNode = new GeneratorNode<T>(targetCount: 1, customConstructor: null);
            // _objectFactory.AddRootNode(_rootNode);
            
            _customizeFactoryOptions = new CustomizeFactoryOptions<T>();
            _objectFactory = new ObjectFactory<T>(_customizeFactoryOptions);
            Customize(_customizeFactoryOptions);
            SetRootNode(instanceCount: 1);
            // Customize(new CustomizeFactoryOptions<T>());
        }
        
        private void SetRootNode(int instanceCount)
        {
            // _currentNode = new GeneratorNode<T>(instanceCount, _customizeFactoryOptions.CustomConstructor);
            _currentNode = new GeneratorNode<T>(instanceCount);
            _objectFactory.AddRootNode(_currentNode);
        }

        protected abstract void Customize(ICustomizeFactoryOptions<T> customization);

        public ISimpleFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            _customizeFactoryOptions.UpdateConstructor(customConstructor);
            // _customConstructor = customConstructor;
            // _rootNode.OverrideCustomConstructor(customConstructor);
            return this;
        }

        // internal void AddDefaultTransform<TValue>(Func<T, TValue> setMember)
        // {
        //     _objectFactory.AddDefaultTransform(new FuncTransform<T,TValue>(setMember.Invoke));
        // }

        public ISimpleFactory<T> WithBehavior(Behavior behavior)
        {
            _customizeFactoryOptions.UpdateBehavior(behavior);
            // var transforms = behavior.GetTransforms<T>();
            // foreach (var transform in transforms)
            // {
            //     _objectFactory.AddDefaultTransform(transform);
            // }
            return this;
        }

        public ICustomizeOneBuildOne<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        ICustomizeOneBuildOneWithNavigation<T> ICustomizeOneBuildOneWithNavigation<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        public ICustomizeOneBuildOneWithNavigation<T> One()
        {
            return this;
        }

        public ICustomizeManyBuildMany<T> Many(int count)
        {
            SetRootNode(instanceCount: count);
            return this;
        }

        public ICustomizeOneBuildManyWithNavigation<T> PlusOne()
        {
            // var newNode = new GeneratorNode<T>(instanceCount: 1, _customizeFactoryOptions.CustomConstructor);
            var newNode = new GeneratorNode<T>(instanceCount: 1);
            _objectFactory.AddNode(newNode);
            
            return this;
        }

        public ICustomizeManyBuildMany<T> Plus(int count)
        {
            // var newNode = new GeneratorNode<T>(instanceCount: count, _customizeFactoryOptions.CustomConstructor);
            var newNode = new GeneratorNode<T>(instanceCount: count);
            _objectFactory.AddNode(newNode);

            return this;
        }

        public T Build()
        {
            return _objectFactory.Build().First();
        }

        ICustomizeOneBuildManyWithNavigation<T> ICustomizeOneBuildManyWithNavigation<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        ICustomizeManyBuildMany<T> ICustomizeManyBuildMany<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        public ICustomizeManyBuildMany<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToFirstNInstances(count, setMember);
            return this;
        }

        public ICustomizeManyBuildMany<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToLastNInstances(count, setMember);
            return this;
        }

        IEnumerable<T> IBuildMany<T>.Build()
        {
            return BuildObjects();
        }

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
                node => new ApplyTransformToFirstInstancesSpecification(count, node.InstanceCount)
            );
        }

        private void AddTransformThatAppliesToLastNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectFactory.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new ApplyTransformToLastInstancesSpecification(count, node.InstanceCount)
            );
        }

        private IEnumerable<T> BuildObjects()
        {
            return _objectFactory.Build();
        }
    }
}