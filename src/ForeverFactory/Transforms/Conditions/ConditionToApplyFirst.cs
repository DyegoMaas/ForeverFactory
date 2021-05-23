using ForeverFactory.Transforms.Conditions.ExecutionContext;

namespace ForeverFactory.Transforms.Conditions
{
    internal class ConditionToApplyFirst : ConditionToApply
    {
        public ConditionToApplyFirst(int count, IExecutionContext executionContext) 
            : base(count, executionContext)
        {
        }

        public override bool CanApplyFor(int index)
        {
            return index < CountToApply;
        }
    }
}