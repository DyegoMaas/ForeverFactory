using System;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators.Transforms.Factories.ReflectionTargets;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithSequentialValuesTransformFactory : BaseRecursiveTransformFactory
    {
        public static DateTime StartingDateTime = new DateTime(
            year: 1753, month: 1, day: 1, 
            hour: 0, minute: 0, second: 0,
            kind: DateTimeKind.Utc
        );

        public FillWithSequentialValuesTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(TargetInfo targetInfo, int index)
        {
            var sequentialNumber = index + 1;
            
            if (targetInfo.TargetType == typeof(string)) 
                return () => targetInfo.Name + sequentialNumber;
            
            if (targetInfo.TargetType == typeof(byte))
                return () =>
                {
                    if (sequentialNumber > byte.MaxValue)
                        return (byte)(sequentialNumber % byte.MaxValue);
                    return (byte)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(short))
                return () =>
                {
                    if (sequentialNumber > short.MaxValue)
                        return (short)(sequentialNumber % short.MaxValue);
                    return (short)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(ushort))
                return () =>
                {
                    if (sequentialNumber > ushort.MaxValue)
                        return (ushort)(sequentialNumber % ushort.MaxValue);
                    return (ushort)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(int))
                return () => sequentialNumber;
            
            if (targetInfo.TargetType == typeof(uint))
                return () => (uint)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(long))
                return () => sequentialNumber;
            
            if (targetInfo.TargetType == typeof(ulong))
                return () => (ulong)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(float))
                return () => sequentialNumber;
            
            if (targetInfo.TargetType == typeof(double))
                return () => sequentialNumber;
            
            if (targetInfo.TargetType == typeof(decimal))
                return () => Convert.ToDecimal(sequentialNumber);

            if (targetInfo.TargetType == typeof(DateTime))
            {
                var increment = index;

                switch (Options.DateTimeIncrements)
                {
                    case DateTimeIncrements.Hours:
                        return () => StartingDateTime.AddHours(increment);
                    case DateTimeIncrements.Minutes:
                        return () => StartingDateTime.AddMinutes(increment);
                    case DateTimeIncrements.Seconds:
                        return () => StartingDateTime.AddSeconds(increment);
                    case DateTimeIncrements.Milliseconds:
                        return () => StartingDateTime.AddMilliseconds(increment);
                    case DateTimeIncrements.Ticks:
                        return () => StartingDateTime.AddTicks(increment);
                    case DateTimeIncrements.Years:
                        return () => StartingDateTime.AddYears(increment);
                    case DateTimeIncrements.Months:
                        return () => StartingDateTime.AddMonths(increment);
                    case DateTimeIncrements.Days:
                    default:
                        return () => StartingDateTime.AddDays(increment);
                }
            }

            return null;
        }
    }
}