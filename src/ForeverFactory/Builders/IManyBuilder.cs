using System;
using System.Collections.Generic;

namespace ForeverFactory.Builders
{
    /// <summary>
    /// This interface allows building many customized objects of type "T".
    /// </summary>
    /// <typeparam name="T">The type of objects that will be built.</typeparam>
    public interface IManyBuilder<out T> : ILinkedBuilder<T>
    {
        /// <summary>
        /// Defines the default value of a property for all instances.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example></param>
        IManyBuilder<T> With<TValue>(Func<T, TValue> setMember);
        
        /// <summary>
        /// Defines the default value of a property for the first N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example></param>
        IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember);
        
        /// <summary>
        /// Defines the default value of a property for the last N instances.
        /// </summary>
        /// <param name="count">How many instances will have this property set.</param>
        /// <param name="setMember">Sets the value of a Property. <example>x => x.Name = "Karen"</example></param>
        IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember);
        
        /// <summary>
        /// Creates a new set of customizable objects, following the previous sets created used the "Many" or "Plus" methods.
        /// </summary>
        /// <param name="count">The number of objects to be created.<param>
        IManyBuilder<T> Plus(int count);
        
        /// <summary>
        /// Builds all the objects configured, including all sets created used the "Many" or "Plus" methods. 
        /// </summary>
        /// <returns>A collection of instances of "T", with all configurations applied.</returns>
        IEnumerable<T> Build();

        ILinkedOneBuilder<T> PluOne();
    }
}