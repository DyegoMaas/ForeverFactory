namespace ForeverFactory.Builders
{
    public interface IDeepNavigation<out T>
    {
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