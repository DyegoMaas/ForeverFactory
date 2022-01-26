using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Behaviors
{
    /// <summary>
    /// This behavior do not fill automatically any properties. After instantiation, all properties of the object
    /// will have default values.
    /// </summary>
    public class DoNotFillBehavior : Behavior
    {
        public override IEnumerable<Transform<T>> GetTransforms<T>()
        {
            return Enumerable.Empty<Transform<T>>();
        }
    }
}