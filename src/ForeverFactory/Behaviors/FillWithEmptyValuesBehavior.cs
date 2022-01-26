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
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            yield return new FillWithEmptyStringTransformFactory().GetTransform<T>();
        }
    }
}