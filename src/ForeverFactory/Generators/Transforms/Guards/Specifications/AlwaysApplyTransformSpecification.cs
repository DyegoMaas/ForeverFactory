namespace ForeverFactory.Generators.Transforms.Guards.Specifications
{
    internal class AlwaysApplyTransformSpecification : CanApplyTransformSpecification
    {
        public override bool CanApply(int index)
        {
            return true;
        }
    }
}