using System;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithSequentialValuesTransformFactory : BaseRecursiveTransformFactory
    {
        public FillWithSequentialValuesTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(PropertyInfo propertyInfo, int index)
        {
            var sequentialNumber = index + 1;
            
            if (propertyInfo.PropertyType == typeof(string)) 
                return () => propertyInfo.Name + sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(byte))
                return () =>
                {
                    if (sequentialNumber > byte.MaxValue)
                        return (byte)(sequentialNumber % byte.MaxValue);
                    return (byte)sequentialNumber;
                };
            
            if (propertyInfo.PropertyType == typeof(short))
                return () =>
                {
                    if (sequentialNumber > short.MaxValue)
                        return (short)(sequentialNumber % short.MaxValue);
                    return (short)sequentialNumber;
                };
            
            if (propertyInfo.PropertyType == typeof(ushort))
                return () =>
                {
                    if (sequentialNumber > ushort.MaxValue)
                        return (ushort)(sequentialNumber % ushort.MaxValue);
                    return (ushort)sequentialNumber;
                };
            
            if (propertyInfo.PropertyType == typeof(int))
                return () => sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(uint))
                return () => (uint)sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(long))
                return () => sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(ulong))
                return () => (ulong)sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(float))
                return () => sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(double))
                return () => sequentialNumber;
            
            if (propertyInfo.PropertyType == typeof(decimal))
                return () => Convert.ToDecimal(sequentialNumber);

            return null;
        }
    }
}