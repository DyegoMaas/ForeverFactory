using System;

namespace ForeverFactory.Builders
{
    public interface ICustomizeOneBuildManyWithNavigation<out T> : IBuildMany<T>, INavigable<T>
    {
        /// <summary>
        ///     Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizeOneBuildManyWithNavigation<T> With<TValue>(Func<T, TValue> setMember);
    }
}