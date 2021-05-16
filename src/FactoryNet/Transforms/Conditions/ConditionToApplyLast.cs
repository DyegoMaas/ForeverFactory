namespace FactoryNet.Transforms.Conditions
{
    public class ConditionToApplyLast : ConditionToApply
    {
        public ConditionToApplyLast(int countToApply, int setSize) 
            : base(countToApply, setSize)
        {
        }

        public override bool CanApplyFor(int index)
        {
            var firstToApply = SetSize - CountToApply;
            return index >= firstToApply;
        }
    }
}