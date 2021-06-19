using ForeverFactory.Core.Transforms.Guards.Specifications;

namespace ForeverFactory.Core.Transforms.Guards
{
    internal class GuardedTransform<T> 
        where T : class
    {
        public Transform<T> Transform { get; }
        public ApplyTransformGuardSpecification Guard { get; }

        public GuardedTransform(Transform<T> transform, ApplyTransformGuardSpecification guard)
        {
            Transform = transform;
            Guard = guard;
        }
    }
}