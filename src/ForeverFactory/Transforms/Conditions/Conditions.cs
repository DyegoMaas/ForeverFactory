using ForeverFactory.Transforms.Conditions.ExecutionContext;

namespace ForeverFactory.Transforms.Conditions
{
    internal static class Conditions
    {
        public static NoConditionToApply NoConditions() => new NoConditionToApply();
        public static ConditionToApply ToApplyFirst(int count, IExecutionContext executionContext) 
            => new ConditionToApplyFirst(count, executionContext);
        public static ConditionToApply ToApplyLast(int count, IExecutionContext executionContext) 
            => new ConditionToApplyLast(count, executionContext);
    }
}