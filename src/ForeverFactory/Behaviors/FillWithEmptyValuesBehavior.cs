using System;
using System.Collections.Generic;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Factories;

namespace ForeverFactory.Behaviors
{
    /// <summary>
    /// This behavior will automatically set all string properties do "".
    /// Nested types will also be recursively initialized.
    /// </summary>
    public class FillWithEmptyValuesBehavior : Behavior
    {
        private readonly RecursiveTransformFactoryOptions _recursiveTransformFactoryOptions;

        public FillWithEmptyValuesBehavior(Action<FillWithEmptyValuesBehaviorOptions> options = null)
        {
            var configuration = new FillWithEmptyValuesBehaviorOptions();
            options?.Invoke(configuration);

            _recursiveTransformFactoryOptions = new RecursiveTransformFactoryOptions
            {
                EnableRecursiveInstantiation = configuration.Recursive,
                FillNullables = configuration.FillNullables
            };
        }

        internal override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            var factory = new FillWithEmptyValuesTransformFactory(_recursiveTransformFactoryOptions);
            yield return factory.GetTransform<T>();
        }
    }

    public class FillWithEmptyValuesBehaviorOptions
    {
        /// <summary>
        /// If enabled, nested classes will be instantiated and all its properties will be set with "".
        /// Default is true.
        /// </summary>
        public bool Recursive { get; set; } = true;

        /// <summary>
        /// If enabled, nullable fields and classes will be filled the same way a normal field or property would.
        /// Default is true.
        /// </summary>
        public bool FillNullables { get; set; } = true;
    }
}