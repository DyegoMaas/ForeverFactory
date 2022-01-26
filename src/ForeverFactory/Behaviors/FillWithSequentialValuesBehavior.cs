using System.Collections.Generic;
using System.Linq;
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
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            yield return new FillWithSequentialValuesTransformFactory().GetTransform<T>();
        }
    }
}