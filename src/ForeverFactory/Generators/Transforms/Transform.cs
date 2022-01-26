namespace ForeverFactory.Generators.Transforms
{
    public abstract class Transform<T>
    {
        public abstract void ApplyTo(T instance, int index = 0);
    }
}