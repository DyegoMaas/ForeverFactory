namespace FactoryNet.Transforms.Conditions
{
    public abstract class ConditionToApply
    {
        public int CountToApply { get; }
        public int SetSize { get; }

        public ConditionToApply(int countToApply, int setSize) // TODO maybe it should receive an IExecutionContext instead of the setSize
        {
            CountToApply = countToApply;
            SetSize = setSize;
        }

        public abstract bool CanApplyFor(int index);
    }
}