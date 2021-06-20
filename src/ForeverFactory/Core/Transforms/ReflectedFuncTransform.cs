using System;

namespace ForeverFactory.Core.Transforms
{
    internal class ReflectedFuncTransform<T> : Transform<T>
    {
        private readonly Func<T, object> _setMember;

        public ReflectedFuncTransform(Func<T, object> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember.Invoke(instance);
        }
    }
}