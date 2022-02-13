namespace ForeverFactory.Behaviors
{
    public interface IOptions<T> : IOptions
        where T : Behavior, new()
    {
    }
    
    public interface IOptions
    {
    }
}