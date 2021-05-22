namespace ForeverFactory.Builders
{
    internal abstract class LinkedBaseBuilder<T> : BaseBuilder<T>
        where T : class
    {
        protected ILinkedBuilder<T> Previous { get; }

        protected LinkedBaseBuilder(ILinkedBuilder<T> previous)
        {
            Previous = previous;
        }
    }
}