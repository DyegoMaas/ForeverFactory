using ForeverFactory.Core.Transforms.Guards.Specifications;

namespace ForeverFactory.Core.Transforms.Guards
{
    internal class NotGuardedTransform<T> : GuardedTransform<T>
        where T : class
    {
        public NotGuardedTransform(Transform<T> transform) 
            : base(transform, new AlwaysApplyTransformGuardSpecification())
        {
        }
    }
}