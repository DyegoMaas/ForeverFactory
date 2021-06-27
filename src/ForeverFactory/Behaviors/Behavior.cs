using System.Collections.Generic;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Behaviors
{
    public abstract class Behavior
    {
        public abstract IEnumerable<Transform<T>> GetTransforms<T>()
            where T : class;
    }
}