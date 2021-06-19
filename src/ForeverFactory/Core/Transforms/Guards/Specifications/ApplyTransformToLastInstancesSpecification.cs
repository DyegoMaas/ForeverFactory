namespace ForeverFactory.Core.Transforms.Guards.Specifications
{
    internal class ApplyTransformToLastInstancesSpecification : ApplyTransformGuardSpecification
    {
        private readonly int _countToApply;
        private readonly int _targetCount;

        public ApplyTransformToLastInstancesSpecification(int countToApply, int targetCount)
        {
            _countToApply = countToApply;
            _targetCount = targetCount;
        }
        
        public override bool CanApply(int currentIndex)
        {
            var firstToApply = _targetCount - _countToApply;
            return currentIndex >= firstToApply;
        }
    }
}