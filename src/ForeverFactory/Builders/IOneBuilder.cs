using System;

namespace ForeverFactory.Builders
{
    /// <summary>
    ///     This interface allows building one customized object of type "T".
    /// </summary>
    /// <typeparam name="T">The type of object that will be built.</typeparam>
    public interface IOneBuilder<out T>
    {
        /// <summary>
        ///     Defines the default value of a property.
        /// </summary>
        /// <param name="setMember">Sets the value of a Property.
        ///     <example>x => x.Name = "Karen"</example>
        ///     >
        /// </param>
        IOneBuilder<T> With<TValue>(Func<T, TValue> setMember);

        /// <summary>
        ///     Applies all configurations and builds a new object of type "T".
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        T Build();

        /// <summary>
        ///     Creates a new builder of "T". It will build a new object, in addition to the previous configurations.
        /// </summary>
        /// <returns>A builder of "T"</returns>
        ILinkedOneBuilder<T> PlusOne();

        /// <summary>
        ///     Creates a new set of customizable objects, following the previous sets created used the "Many" or "Plus" methods.
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        IManyBuilder<T> Plus(int count);
    }
}