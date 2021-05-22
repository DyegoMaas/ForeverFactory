using System;
using System.Collections.Generic;

namespace ForeverFactory.Builders
{
    public interface ILinkedOneBuilder<out T> : ILinkedBuilder<T>
    {
        // TODO support custom constructor
        
        /// <summary>
        /// Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example>></param>
        ILinkedOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
        
        /// <summary>
        /// Applies all configurations and builds a new object of type "T". 
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        IEnumerable<T> Build();

        ILinkedOneBuilder<T> PlusOne();
    }
}