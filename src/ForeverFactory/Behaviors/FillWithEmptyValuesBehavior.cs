using System.Collections.Generic;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Factories;

namespace ForeverFactory.Behaviors
{
    public class FillWithEmptyValuesBehavior : Behavior
    {
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            yield return TransformFactory.FillWithEmptyValues<T>();
        }
    }
}