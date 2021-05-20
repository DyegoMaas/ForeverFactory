namespace ForeverFactory
{
    /// <summary>
    /// A customizable factory of objects of type "T".
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public sealed class DefaultFactory<T> : MagicFactory<T>
        where T : class
    {
    }
}