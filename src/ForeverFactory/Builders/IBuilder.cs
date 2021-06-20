using System.Collections.Generic;

namespace ForeverFactory.Builders
{
    public interface IBuilder<out T>
    {
        /// <summary>
        ///     Builds all the objects configured, including all sets created used the "Many" or "Plus" methods.
        /// </summary>
        /// <returns>A collection of instances of "T", with all configurations applied.</returns>
        IEnumerable<T> Build();
    }
}