using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Core.Transforms;

namespace ForeverFactory.Behaviors
{
    public class DoNotFillBehavior : Behavior
    {
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            return Enumerable.Empty<Transform<T>>();
        }
    }
}