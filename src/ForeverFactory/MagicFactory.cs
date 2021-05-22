using System;
using System.Collections.Generic;
using ForeverFactory.Builders;
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
    }

    /// <summary>
    /// A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory<T> : IOneBuilder<T>
        where T : class
    {
        private readonly OneBuilder<T> _oneBuilder = new OneBuilder<T>();
        private readonly TransformList<T> _defaultTransforms = new TransformList<T>();
        private Func<T> _customConstructor;

        /// <summary>
        /// Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        protected void UseConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _oneBuilder.SetCustomConstructor(customConstructor);
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
            _oneBuilder.SetDefault(setMember);
        }

        # region OneBuilder Wrapper
        
        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _oneBuilder.With(setMember);
            return _oneBuilder;
        }

        /// <summary>
        /// Applies all configurations and builds a new object of type "T". 
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        public T Build()
        {
            return _oneBuilder.Build();
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(
                defaultTransforms: new TransformList<T>(),
                previous: new MagicFactoryOneBuilderToLinkedOneBuilderAdapter(this, _defaultTransforms, _customConstructor)
            );
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count,
                sharedContext: new SharedContext(_defaultTransforms,_customConstructor                ),
                previousBuilder: new MagicFactoryOneBuilderToLinkedOneBuilderAdapter(this, _defaultTransforms, _customConstructor)
            );
        }

        private class SharedContext : ISharedContext<T>
        {
            public TransformList<T> DefaultTransforms { get; }
            public Func<T> CustomConstructor { get; set; }

            public SharedContext(TransformList<T> defaultTransforms, Func<T> customConstructor)
            {
                DefaultTransforms = defaultTransforms;
                CustomConstructor = customConstructor;
            }
        }
        
        private class MagicFactoryOneBuilderToLinkedOneBuilderAdapter : ILinkedBuilder<T> 
        {
            private readonly IOneBuilder<T> _builder;
            private readonly TransformList<T> _defaultTransforms;
            private readonly Func<T> _customConstructor;

            public MagicFactoryOneBuilderToLinkedOneBuilderAdapter(
                MagicFactory<T> builder,
                TransformList<T> defaultTransforms,
                Func<T> customConstructor)
            {
                _builder = builder;
                _defaultTransforms = defaultTransforms;
                _customConstructor = customConstructor;
            }

            public ILinkedOneBuilder<T> PlusOne()
            {
                return new LinkedOneBuilder<T>(_defaultTransforms, this);
            }

            public IManyBuilder<T> Plus(int count)
            {
                return new ManyBuilder<T>(count, 
                    new SharedContext(_defaultTransforms, _customConstructor), 
                    previousBuilder: null
                );
            }

            public IEnumerable<T> Build()
            {
                yield return _builder.Build();
            }
        }

        #endregion

        /// <summary>
        /// Allows to build multiple objects. This method gives access to further group customization.
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        /// <returns>A builder for multiple objects.</returns>
        public IManyBuilder<T> Many(int count)
        {
            return new ManyBuilder<T>(count, new SharedContext(_defaultTransforms, _customConstructor));
        }
    }
}