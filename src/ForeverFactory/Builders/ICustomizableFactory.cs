using System;
using ForeverFactory.Behaviors;

namespace ForeverFactory.Builders
{
    public interface ICustomizableFactory<T> : IOneBuilder<T>
        where T : class
    {
        /// <summary>
        ///     Configures this factory to instantiate the object of type "T" using this constructor.
        /// </summary>
        /// <param name="customConstructor">Constructor used to build "T" objects</param>
        ICustomizableFactory<T> UsingConstructor(Func<T> customConstructor);

        /// <summary>
        ///     Defines the behavior used to fill the properties of a class.
        ///     DoNotFillBehavior is used by default, but you can selected other behaviors tool. 
        /// </summary>
        /// <see cref="DoNotFillBehavior"/>
        /// <see cref="FillWithEmptyValuesBehavior"/>
        /// <param name="behavior">The type of the behavior to be used</param>
        ICustomizableFactory<T> WithBehavior(Behavior behavior);

        /// <summary>
        ///     Creates a set of customizable objects
        /// </summary>
        /// <param name="count">The number of objects to be created.</param>
        IManyBuilder<T> Many(int count);
    }
}