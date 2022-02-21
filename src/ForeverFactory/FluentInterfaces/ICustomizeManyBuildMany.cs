using System;

namespace ForeverFactory.FluentInterfaces
{
    /// <summary>
    ///     This interface allows building many customized objects of type "T".
    /// </summary>
    /// <typeparam name="T">The type of objects that will be built.</typeparam>
    public interface ICustomizeManyBuildMany<out T> : IBuildMany<T>, INavigable<T>
    {
        /// <summary>
        ///     Defines the value of a property for all instances.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizeManyBuildMany<T> With<TValue>(Func<T, TValue> setMember);

        /// <summary>
        ///     Defines the value of a property for the first N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizeManyBuildMany<T> WithFirst<TValue>(int count, Func<T, TValue> setMember);

        /// <summary>
        ///     Defines the value of a property for the last N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizeManyBuildMany<T> WithLast<TValue>(int count, Func<T, TValue> setMember);
    }
}