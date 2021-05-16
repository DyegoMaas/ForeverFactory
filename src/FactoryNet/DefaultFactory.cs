namespace FactoryNet
{
    public class DefaultFactory<T> : MagicFactory<T>
        where T : new()
    {
    }
}