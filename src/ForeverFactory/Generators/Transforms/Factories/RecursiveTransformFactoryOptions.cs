using ForeverFactory.Behaviors;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class RecursiveTransformFactoryOptions
    {
        public bool EnableRecursiveInstantiation { get; set; } = true;
        public DateTimeIncrements DateTimeIncrements { get; set; } = DateTimeIncrements.Days;
    }
}