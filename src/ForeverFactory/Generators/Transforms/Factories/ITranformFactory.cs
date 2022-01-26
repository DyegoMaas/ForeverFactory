namespace ForeverFactory.Generators.Transforms.Factories
{
    internal interface ITranformFactory
    {
        Transform<T> GetTransform<T>() 
            where T : class;
    }
}