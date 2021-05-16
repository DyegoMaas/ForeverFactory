using System;

namespace FactoryNet
{
    public sealed class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly Func<T, TValue> _setMember;

        public FuncTransform(Func<T, TValue> setMember, ConditionToApply conditionToApply = null) 
            : base(conditionToApply)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember(instance);
        }
    }

    public class ConditionToApply
    {
        public Condition Condition { get; }
        public int Count { get; }

        public ConditionToApply(Condition condition, int count)
        {
            Condition = condition;
            Count = count;
        }
    }

    public enum Condition
    {
        First,
        Last
    }
}