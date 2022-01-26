namespace ForeverFactory.Generators.Transforms.Factories
{
    internal interface ITranformFactory
    {
        Transform<T> GetTransformers<T>() 
            where T : class;
    }
}