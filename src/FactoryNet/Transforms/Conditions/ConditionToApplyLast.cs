using FactoryNet.ExecutionContext;

namespace FactoryNet.Transforms.Conditions
{
    internal class ConditionToApplyLast : ConditionToApply
    {
        public ConditionToApplyLast(int count, IExecutionContext executionContext) 
            : base(count, executionContext)
        {
        }

        public override bool CanApplyFor(int index)
        {
            var firstToApply = ExecutionContext.QuantityToProduce - CountToApply;
            return index >= firstToApply;
        }
    }
}