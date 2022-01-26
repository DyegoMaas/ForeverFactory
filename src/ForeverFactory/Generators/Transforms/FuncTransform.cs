namespace ForeverFactory.Generators.Transforms
{
    internal delegate TAffectedProperty SetMember<in T, out TAffectedProperty>(T arg);

    internal class FuncTransform<T, TValue> : Transform<T>
    {
        private readonly SetMember<T, TValue> _setMember;

        public FuncTransform(SetMember<T, TValue> setMember)
        {
            _setMember = setMember;
        }

        public override void ApplyTo(T instance, int index = 0)
        {
            _setMember.Invoke(instance);
        }
    }
}