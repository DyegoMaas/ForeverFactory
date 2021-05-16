using System;

namespace FactoryNet
{
    public sealed class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly Func<T, TValue> _setMember;

        public FuncTransform(Func<T, TValue> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember(instance);
        }
    }
}