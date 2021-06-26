using System;

namespace ForeverFactory.Builders
{
    public interface ILinkedOneBuilder<out T> : IMultipleInstanceBuilder<T>, INavigatable<T>
    {
        /// <summary>
        ///     Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        ///     >
        /// </param>
        ILinkedOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
    }
}