using System;
using System.Collections.Generic;
using ForeverFactory.Builders;
using ForeverFactory.Builders.Adapters;
using ForeverFactory.Builders.Common;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

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
        public static MagicFactory<T> For<T>() where T : class => new DefaultFactory<T>();
        
        private sealed class DefaultFactory<T> : MagicFactory<T>
            where T : class
        {
        }
    }

    /// <summary>
    /// A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory<T> : IOneBuilder<T>
        where T : class
    {
        private readonly TransformList<T> _defaultTransforms = new TransformList<T>();
        private readonly List<Transform<T>> _transforms = new List<Transform<T>>();
        private Func<T> _customConstructor;

        /// <summary>
        /// Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        protected void UseConstructor(Func<T> customConstructor) // TODO test
        {
            _customConstructor = customConstructor;
        }

        /// <summary>
        /// Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        public MagicFactory<T> UsingConstructor(Func<T> customConstructor)
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
            _defaultTransforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
        }

        # region OneBuilder Wrapper
        
        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        /// <summary>
        /// Applies all configurations and builds a new object of type "T". 
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        public T Build()
        {
            var oneBuilder = new OneBuilder<T>(new SharedContext<T>(_defaultTransforms, _customConstructor));
            foreach (var transform in _transforms)
            {
                oneBuilder.With(transform);
            }
            return oneBuilder.Build();
        }

        /// <summary>
        /// Allows to build multiple objects. This method gives access to further group customization.
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        /// <returns>A builder for multiple objects.</returns>
        public IManyBuilder<T> Many(int count)
        {
            return new LinkedManyBuilder<T>(count, new SharedContext<T>(_defaultTransforms, _customConstructor));
        }

        // TODO document
        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(
                sharedContext: new SharedContext<T>(_defaultTransforms,_customConstructor),
                previous: new MagicFactoryOneBuilderToLinkedOneBuilderAdapter<T>(this, _defaultTransforms, _customConstructor)
            );
        }

        // TODO document
        public IManyBuilder<T> Plus(int count)
        {
            return new LinkedManyBuilder<T>(count,
                sharedContext: new SharedContext<T>(_defaultTransforms,_customConstructor                ),
                previous: new MagicFactoryOneBuilderToLinkedOneBuilderAdapter<T>(this, _defaultTransforms, _customConstructor)
            );
        }

        #endregion
    }
}