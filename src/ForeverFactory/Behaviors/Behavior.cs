using System.Collections.Generic;
using ForeverFactory.Generators.Transforms;

namespace ForeverFactory.Behaviors
{
    /// <summary>
    /// Base class for Behaviors you can use to customize how Forever Factory builds objects for you.
    /// </summary>
    public abstract class Behavior
    {
        public abstract IEnumerable<Transform<T>> GetTransforms<T>()
            where T : class;
    }
}