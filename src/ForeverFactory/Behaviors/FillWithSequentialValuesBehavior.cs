using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Factories;

namespace ForeverFactory.Behaviors
{
    internal class FillWithSequentialValuesBehavior : Behavior
    {
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            yield return new FillWithSequentialValuesTransformFactory().GetTransform<T>();
        }
    }
}