using ForeverFactory.Builders.Common;

namespace ForeverFactory.Builders
{
    internal abstract class LinkedBaseBuilder<T> : BaseBuilder<T>
        where T : class
    {
        protected ILinkedBuilder<T> Previous { get; }

        protected LinkedBaseBuilder(ISharedContext<T> sharedContext, ILinkedBuilder<T> previous) 
            : base(sharedContext) 
        {
            Previous = previous;
        }
    }
}