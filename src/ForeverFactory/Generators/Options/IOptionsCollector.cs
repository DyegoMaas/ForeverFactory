namespace ForeverFactory.Generators.Options
{
    internal interface IOptionsCollector<T>
        where T : class
    {
        IObjectFactoryOptions<T> Collect();
    }
}