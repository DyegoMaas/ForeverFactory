using System;

namespace ForeverFactory.FluentInterfaces
{
    /// <summary>
    ///     This interface allows building one customized object of type "T".
    /// </summary>
    /// <typeparam name="T">The type of object that will be built.</typeparam>
    public interface ICustomizeOneBuildOne<out T> : IBuildOne<T>
    {
        /// <summary>
        ///     Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        ///     >
        /// </param>
        ICustomizeOneBuildOne<T> With<TValue>(Func<T, TValue> setMember);
    }
}