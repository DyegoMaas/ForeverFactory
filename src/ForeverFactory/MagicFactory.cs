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
        private readonly ObjectBuilder<T> _objectBuilder;
        private readonly OptionsCollector<T> _optionsCollector;

        protected MagicFactory()
        {
            _optionsCollector = new OptionsCollector<T>(customize: Customize);
            _objectBuilder = new ObjectBuilder<T>(_optionsCollector);
            SetRootNode(instanceCount: 1);
        }

        protected abstract void Customize(ICustomizeFactoryOptions<T> customization);

        private void SetRootNode(int instanceCount)
        {
            var rootNode = new GeneratorNode<T>(instanceCount);
            _objectBuilder.AddRootNode(rootNode);
        }

        public ISimpleFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            _optionsCollector.UpdateConstructor(customConstructor);
            return this;
        }

        public ISimpleFactory<T> WithBehavior(Behavior behavior)
        {
            _optionsCollector.UpdateBehavior(behavior);
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
            var newNode = new GeneratorNode<T>(instanceCount: 1);
            _objectBuilder.AddNode(newNode);
            
            return this;
        }

        public ICustomizeManyBuildMany<T> Plus(int count)
        {
            var newNode = new GeneratorNode<T>(instanceCount: count);
            _objectBuilder.AddNode(newNode);

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

        ICustomizeOneBuildManyWithNavigation<T> ICustomizeOneBuildManyWithNavigation<T>.Do(Action<T> callback)
        {
            AddTransformThatAlwaysApply(WrapAction(callback));
            return this;
        }

        ICustomizeOneBuildOneWithNavigation<T> ICustomizeOneBuildOneWithNavigation<T>.Do(Action<T> callback)
        {
            AddTransformThatAlwaysApply(WrapAction(callback));
            return this;
        }

        ICustomizeOneBuildOne<T> ICustomizeOneBuildOne<T>.Do(Action<T> callback)
        {
            AddTransformThatAlwaysApply(WrapAction(callback));
            return this;
        }

        ICustomizeManyBuildMany<T> ICustomizeManyBuildMany<T>.Do(Action<T> callback)
        {
            AddTransformThatAlwaysApply(WrapAction(callback));
            return this;
        }

        public T Build()
        {
            return _objectBuilder.Build().First();
        }

        IEnumerable<T> IBuildMany<T>.Build()
        {
            return _objectBuilder.Build();
        }

        private void AddTransformThatAppliesToFirstNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectBuilder.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new ApplyTransformToFirstInstancesSpecification(count, node.InstanceCount)
            );
        }

        private void AddTransformThatAppliesToLastNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            _objectBuilder.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new ApplyTransformToLastInstancesSpecification(count, node.InstanceCount)
            );
        }

        private void AddTransformThatAlwaysApply<TValue>(Func<T, TValue> setMember)
        {
            _objectBuilder.AddTransform(
                new FuncTransform<T, TValue>(setMember.Invoke),
                node => new AlwaysApplyTransformSpecification()
            );
        }

        private Func<T, bool> WrapAction(Action<T> callback)
        {
            bool Wrapper(T instance)
            {
                callback.Invoke(instance);
                return true;
            }

            return Wrapper;
        }
    }
}