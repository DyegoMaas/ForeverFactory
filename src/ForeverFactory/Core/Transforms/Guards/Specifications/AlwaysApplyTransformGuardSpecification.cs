namespace ForeverFactory.Core.Transforms.Guards.Specifications
{
    internal class AlwaysApplyTransformGuardSpecification : ApplyTransformGuardSpecification
    {
        public override bool CanApply(int currentIndex) => true;
    }
}