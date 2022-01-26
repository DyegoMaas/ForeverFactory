using System;
using System.Collections.Generic;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Factories;

namespace ForeverFactory.Behaviors
{
    /// <summary>
    /// This behavior will automatically set all properties sequential values.
    /// Nested objects will also be recursively initialized.  
    /// </summary>
    /// <example>
    /// var persons = MagicFactory.For<ClassWithInteger>().Many(100).Build();
    /// person[0].Name == "Name1"
    /// person[0].Age == "1"
    /// person[1].Name == "Name2"
    /// person[1].Age == "2"
    /// ...
    /// </example>
    public class FillWithSequentialValuesBehavior : Behavior
    {
        private readonly RecursiveTransformFactoryOptions _recursiveTransformFactoryOptions;

        public FillWithSequentialValuesBehavior(Action<FillWithSequentialValuesBehaviorOptions> options = null)
        {
            var configuration = new FillWithSequentialValuesBehaviorOptions();
            options?.Invoke(configuration);

            _recursiveTransformFactoryOptions = new RecursiveTransformFactoryOptions
            {
                EnableRecursiveInstantiation = configuration.Recursive
            };
        }
        
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            var factory = new FillWithSequentialValuesTransformFactory(_recursiveTransformFactoryOptions);
            yield return factory.GetTransform<T>();
        }
    }
    
    public class FillWithSequentialValuesBehaviorOptions
    {
        /// <summary>
        /// If enabled, nested classes will be instantiated and all its properties will be set with sequential values.
        /// Default is true.
        /// </summary>
        public bool Recursive { get; set; } = true;
    }
}