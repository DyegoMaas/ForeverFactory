namespace FactoryNet.Transforms.Conditions
{
    public class ConditionToApplyFirst : ConditionToApply
    {
        public ConditionToApplyFirst(int countToApply, int setSize) 
            : base(countToApply, setSize)
        {
        }

        public override bool CanApplyFor(int index)
        {
            return index < CountToApply;
        }
    }
}