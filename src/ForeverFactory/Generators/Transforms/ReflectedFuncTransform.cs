using System;

namespace ForeverFactory.Generators.Transforms
{
    internal class ReflectedFuncTransform<T> : Transform<T>
    {
        private readonly Func<T, int, object> _setMember;

        public ReflectedFuncTransform(Func<T, int, object> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance, int index = 0)
        {
            _setMember.Invoke(instance, index);
        }
    }
}