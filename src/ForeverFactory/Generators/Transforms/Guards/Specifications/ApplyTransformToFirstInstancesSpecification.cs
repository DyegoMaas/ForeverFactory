using System;

namespace ForeverFactory.Generators.Transforms.Guards.Specifications
{
    internal class ApplyTransformToFirstInstancesSpecification : CanApplyTransformSpecification
    {
        private readonly int _countToApply;

        public ApplyTransformToFirstInstancesSpecification(int countToApply, int targetCount)
        {
            if (countToApply < 0)
                throw new ArgumentException($"Not possible to apply to {countToApply}. Only positive values are accepted");
            
            if (countToApply > targetCount)
                throw new ArgumentException($"Not possible to apply to {countToApply}. Max size is {targetCount}.");
            
            _countToApply = countToApply;
        }

        public override bool CanApply(int currentIndex)
        {
            return currentIndex < _countToApply;
        }
    }
}