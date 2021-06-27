using ForeverFactory.Generators.Transforms.Guards.Specifications;

namespace ForeverFactory.Generators.Transforms.Guards
{
    internal class GuardedTransform<T>
        where T : class
    {
        public GuardedTransform(Transform<T> transform, CanApplyTransformSpecification guard)
        {
            Transform = transform;
            Guard = guard;
        }

        public Transform<T> Transform { get; }
        public CanApplyTransformSpecification Guard { get; }
    }
}