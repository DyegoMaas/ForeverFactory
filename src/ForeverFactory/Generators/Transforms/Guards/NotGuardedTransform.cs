using ForeverFactory.Generators.Transforms.Guards.Specifications;

namespace ForeverFactory.Generators.Transforms.Guards
{
    internal class NotGuardedTransform<T> : GuardedTransform<T>
        where T : class
    {
        public NotGuardedTransform(Transform<T> transform)
            : base(transform, new AlwaysApplyTransformSpecification())
        {
        }
    }
}