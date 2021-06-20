namespace ForeverFactory.Core.Transforms.Guards.Specifications
{
    internal class AlwaysApplyTransformSpecification : CanApplyTransformSpecification
    {
        public override bool CanApply(int currentIndex) => true;
    }
}