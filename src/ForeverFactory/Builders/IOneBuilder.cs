using System;

namespace ForeverFactory.Builders
{
    /// <summary>
    /// This interface allows building one customized object of type "T".
    /// </summary>
    /// <typeparam name="T">The type of object that will be built.</typeparam>
    public interface IOneBuilder<out T>
    {
        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
        IOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
        
        /// <summary>
        /// Applies all configurations and builds a new object of type "T". 
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        T Build();
    }
}