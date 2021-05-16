namespace FactoryNet.Transforms.Conditions
{
    public abstract class ConditionToApply
    {
        public int CountToApply { get; }
        public int SetSize { get; }

        public ConditionToApply(int countToApply, int setSize)
        {
            CountToApply = countToApply;
            SetSize = setSize;
        }

        public abstract bool CanApplyFor(int index);
    }
}