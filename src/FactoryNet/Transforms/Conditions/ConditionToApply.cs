using FactoryNet.ExecutionContext;

namespace FactoryNet.Transforms.Conditions
{
    internal abstract class ConditionToApply
    {
        protected int CountToApply { get; }
        protected IExecutionContext ExecutionContext { get; }

        public ConditionToApply(int count, IExecutionContext executionContext)
        {
            CountToApply = count;
            ExecutionContext = executionContext;
        }

        public abstract bool CanApplyFor(int index);
    }
}