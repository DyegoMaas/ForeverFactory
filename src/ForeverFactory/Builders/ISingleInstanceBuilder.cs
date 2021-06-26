namespace ForeverFactory.Builders
{
    public interface ISingleInstanceBuilder<out T>
    {
        /// <summary>
        ///     Applies all configurations and builds a new object of type "T".
        /// </summary>
        /// <returns>A new instance of "T", with all configurations applied.</returns>
        T Build();
    }
}