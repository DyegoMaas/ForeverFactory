namespace ForeverFactory.Core.Transforms
{
    public abstract class Transform<T>
    {
        public abstract void ApplyTo(T instance);
    }
}