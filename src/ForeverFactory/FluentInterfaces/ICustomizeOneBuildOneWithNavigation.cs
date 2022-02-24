using System;

namespace ForeverFactory.FluentInterfaces
{
    public interface ICustomizeOneBuildOneWithNavigation<out T> : IBuildOne<T>, INavigable<T>
    {
        /// <summary>
        ///     Defines the value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        ///     >
        /// </param>
        ICustomizeOneBuildOneWithNavigation<T> With<TValue>(Func<T, TValue> setMember);
        
        // TODO document
        ICustomizeOneBuildOneWithNavigation<T> Do(Action<T> callback);
    }
}