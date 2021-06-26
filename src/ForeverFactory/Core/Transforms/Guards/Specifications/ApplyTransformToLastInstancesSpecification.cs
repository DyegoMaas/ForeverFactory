using System;

namespace ForeverFactory.Core.Transforms.Guards.Specifications
{
    internal class ApplyTransformToLastInstancesSpecification : CanApplyTransformSpecification
    {
        private readonly int _countToApply;
        private readonly int _targetCount;

        public ApplyTransformToLastInstancesSpecification(int countToApply, int targetCount)
        {
            if (countToApply < 0)
                throw new ArgumentException($"Not possible to apply to {countToApply}. Only positive values are accepted");
            
            if (countToApply > targetCount)
                throw new ArgumentException($"Not possible to apply to {countToApply}. Max size is {targetCount}.");
            
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