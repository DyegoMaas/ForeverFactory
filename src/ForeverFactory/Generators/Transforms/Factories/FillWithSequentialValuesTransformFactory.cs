using System;
using ForeverFactory.Behaviors;
using ForeverFactory.Generators.Transforms.Factories.ReflectionTargets;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithSequentialValuesTransformFactory : BaseRecursiveTransformFactory
    {
        public FillWithSequentialValuesTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(TargetInfo targetInfo, int index)
        {
            var sequentialNumber = index + 1;
            
            if (targetInfo.TargetType == typeof(string)) 
                return () => targetInfo.Name + sequentialNumber;

            var nullableUnderlyingType = targetInfo.GetNullableUnderlyingType();
            if (targetInfo.TargetType == typeof(byte) || nullableUnderlyingType == typeof(byte))
                return () =>
                {
                    if (sequentialNumber > byte.MaxValue)
                        return (byte)(sequentialNumber % byte.MaxValue);
                    return (byte)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(short) || nullableUnderlyingType == typeof(short))
                return () =>
                {
                    if (sequentialNumber > short.MaxValue)
                        return (short)(sequentialNumber % short.MaxValue);
                    return (short)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(ushort) || nullableUnderlyingType == typeof(ushort))
                return () =>
                {
                    if (sequentialNumber > ushort.MaxValue)
                        return (ushort)(sequentialNumber % ushort.MaxValue);
                    return (ushort)sequentialNumber;
                };
            
            if (targetInfo.TargetType == typeof(int) || nullableUnderlyingType == typeof(int))
                return () => sequentialNumber;
            
            if (targetInfo.TargetType == typeof(uint) || nullableUnderlyingType == typeof(uint))
                return () => (uint)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(long) || nullableUnderlyingType == typeof(long))
                return () => (long)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(ulong) || nullableUnderlyingType == typeof(ulong))
                return () => (ulong)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(float) || nullableUnderlyingType == typeof(float))
                return () => (float)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(double) || nullableUnderlyingType == typeof(double))
                return () => (double)sequentialNumber;
            
            if (targetInfo.TargetType == typeof(decimal) || nullableUnderlyingType == typeof(decimal))
                return () => Convert.ToDecimal(sequentialNumber);

            if (targetInfo.TargetType == typeof(DateTime) || nullableUnderlyingType == typeof(DateTime))
            {
                var increment = index;

                switch (Options.DateTimeIncrements)
                {
                    case DateTimeIncrements.Hours:
                        return () => Options.StartDate.AddHours(increment);
                    case DateTimeIncrements.Minutes:
                        return () => Options.StartDate.AddMinutes(increment);
                    case DateTimeIncrements.Seconds:
                        return () => Options.StartDate.AddSeconds(increment);
                    case DateTimeIncrements.Milliseconds:
                        return () => Options.StartDate.AddMilliseconds(increment);
                    case DateTimeIncrements.Ticks:
                        return () => Options.StartDate.AddTicks(increment);
                    case DateTimeIncrements.Years:
                        return () => Options.StartDate.AddYears(increment);
                    case DateTimeIncrements.Months:
                        return () => Options.StartDate.AddMonths(increment);
                    case DateTimeIncrements.Days:
                    default:
                        return () => Options.StartDate.AddDays(increment);
                }
            }

            return null;
        }
    }
}