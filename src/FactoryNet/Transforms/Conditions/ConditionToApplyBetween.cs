namespace FactoryNet.Transforms.Conditions
{
    public class ConditionToApplyBetween : ConditionToApply
    {
        public int StartingFromIndex { get; }

        public ConditionToApplyBetween(int countToApply, int startingFromIndex, int setSize) 
            : base(countToApply, setSize)
        {
            StartingFromIndex = startingFromIndex;
        }

        public override bool CanApplyFor(int index)
        {
            return index >= StartingFromIndex && index < StartingFromIndex + CountToApply;
        }
    }
}