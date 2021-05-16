using System;

namespace FactoryNet
{
    public sealed class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly Func<T, TValue> _setMember;

        public FuncTransform(Func<T, TValue> setMember, ConditionToApply conditionToApply) 
            : base(conditionToApply)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember(instance);
        }
    }

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