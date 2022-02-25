using System;

namespace ForeverFactory.FluentInterfaces
{
    public interface ICustomizeOneBuildManyWithNavigation<out T> : IBuildMany<T>, INavigable<T>
    {
        /// <summary>
        ///     Defines the value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        /// </param>
        ICustomizeOneBuildManyWithNavigation<T> With<TValue>(Func<T, TValue> setMember);
        
        /// <summary>
        ///     Executes the callback passing the instance with its current state.
        /// </summary>
        /// <param name="callback">
        ///     <example>x => Console.WriteLine(x.Name)</example>
        /// </param>
        ICustomizeOneBuildManyWithNavigation<T> Do(Action<T> callback);
    }
}