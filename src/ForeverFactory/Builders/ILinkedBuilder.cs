using System.Collections.Generic;

namespace ForeverFactory.Builders
{
    public interface ILinkedBuilder<out T>
    {
        IEnumerable<T> Build();
    }
}