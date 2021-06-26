using System;

namespace ForeverFactory.Builders
{
    /// <summary>
    ///     This interface allows building many customized objects of type "T".
    /// </summary>
    /// <typeparam name="T">The type of objects that will be built.</typeparam>
    public interface ICustomizableMultipleInstanceBuilder<out T> : IMultipleInstanceBuilder<T>, INavigatable<T>
    {
        /// <summary>
        ///     Defines the default value of a property for all instances.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizableMultipleInstanceBuilder<T> With<TValue>(Func<T, TValue> setMember);

        /// <summary>
        ///     Defines the default value of a property for the first N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizableMultipleInstanceBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember);

        /// <summary>
        ///     Defines the default value of a property for the last N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizableMultipleInstanceBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember);
    }
}