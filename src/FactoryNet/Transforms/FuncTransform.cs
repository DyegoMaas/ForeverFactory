using System;
using FactoryNet.Transforms.Conditions;

namespace FactoryNet.Transforms
{
    internal sealed class FuncTransform<T, TValue> : Transform<T>
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
}