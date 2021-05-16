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
        public int Count { get; }
        public int SetSize { get; }

        public ConditionToApply(int count, int setSize)
        {
            Count = count;
            SetSize = setSize;
        }

        public abstract bool CanApplyFor(int index);
    }
    
    public class NoConditionToApply : ConditionToApply
    {
        public NoConditionToApply() 
            : base(count: 0, setSize: 0)
        {
        }

        public override bool CanApplyFor(int index)
        {
            return true;
        }
    }
    
    public class ConditionToApplyFirst : ConditionToApply
    {
        public ConditionToApplyFirst(int count, int setSize) 
            : base(count, setSize)
        {
        }

        public override bool CanApplyFor(int index)
        {
            return index < Count;
        }
    }
    
    public class ConditionToApplyLast : ConditionToApply
    {
        public ConditionToApplyLast(int count, int setSize) 
            : base(count, setSize)
        {
        }

        public override bool CanApplyFor(int index)
        {
            var firstToApply = SetSize - Count;
            return index >= firstToApply;
        }
    }
    
    public class ConditionToApplyBetween : ConditionToApply
    {
        public int StartingFromIndex { get; }

        public ConditionToApplyBetween(int count, int setSize, int startingFromIndex) 
            : base(count, setSize)
        {
            StartingFromIndex = startingFromIndex;
        }

        public override bool CanApplyFor(int index)
        {
            throw new NotImplementedException();
        }
    }
}