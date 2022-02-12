namespace ForeverFactory.Generators
{
    internal interface IOptionsCollector<T>
        where T : class
    {
        IObjectFactoryOptions<T> Initialize();
    }
}