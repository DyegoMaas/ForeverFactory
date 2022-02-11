using System;
using ForeverFactory.Behaviors;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class RecursiveTransformFactoryOptions
    {
        public static DateTime DefaultStartDate = new DateTime(
            year: 1753, month: 1, day: 1, 
            hour: 0, minute: 0, second: 0,
            kind: DateTimeKind.Utc
        );

        public bool EnableRecursiveInstantiation { get; set; } = true;

        public DateTimeIncrements DateTimeIncrements { get; set; } = DateTimeIncrements.Days;

        public DateTime StartDate { get; set; } = DefaultStartDate;
    }
}