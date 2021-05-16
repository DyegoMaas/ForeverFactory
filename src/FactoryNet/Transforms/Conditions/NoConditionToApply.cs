using FactoryNet.ExecutionContext;

namespace FactoryNet.Transforms.Conditions
{
    internal class NoConditionToApply : ConditionToApply
    {
        public NoConditionToApply() 
            : base(count: 0, new InstanceSetExecutionContext(quantityToProduce: 0))
        {
        }

        public override bool CanApplyFor(int index)
        {
            return true;
        }
    }
}