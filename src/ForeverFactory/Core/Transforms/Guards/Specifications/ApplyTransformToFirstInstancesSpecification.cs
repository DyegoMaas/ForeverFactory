namespace ForeverFactory.Core.Transforms.Guards.Specifications
{
    internal class ApplyTransformToFirstInstancesSpecification : CanApplyTransformSpecification
    {
        private readonly int _countToApply;

        public ApplyTransformToFirstInstancesSpecification(int countToApply)
        {
            _countToApply = countToApply;
        }

        public override bool CanApply(int currentIndex)
        {
            return currentIndex < _countToApply;
        }
    }
}