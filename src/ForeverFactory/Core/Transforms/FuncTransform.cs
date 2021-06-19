namespace ForeverFactory.Core.Transforms
{
    internal class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly SetMember<T, TValue> _setMember;

        public FuncTransform(SetMember<T, TValue> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance)
        {
            _setMember.Invoke(instance);
        }
    }
}