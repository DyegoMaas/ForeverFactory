namespace FactoryNet.Transforms.Conditions
{
    public class NoConditionToApply : ConditionToApply
    {
        public NoConditionToApply() 
            : base(countToApply: 0, setSize: 0)
        {
        }

        public override bool CanApplyFor(int index)
        {
            return true;
        }
    }
}