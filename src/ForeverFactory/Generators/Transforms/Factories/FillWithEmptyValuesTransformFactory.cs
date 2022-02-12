using System;
using ForeverFactory.Generators.Transforms.Factories.ReflectionTargets;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithEmptyValuesTransformFactory : BaseRecursiveTransformFactory
    {
        public FillWithEmptyValuesTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(TargetInfo targetInfo, int index)
        {
            if (targetInfo.TargetType == typeof(string)) 
                return () => string.Empty;

            var nullableUnderlyingType = targetInfo.GetNullableUnderlyingType();
            if (targetInfo.TargetType == typeof(DateTime) || nullableUnderlyingType == typeof(DateTime))
                return () => RecursiveTransformFactoryOptions.DefaultStartDate;
            
            if (nullableUnderlyingType == typeof(byte))
                return () => default(byte);
            
            if (nullableUnderlyingType == typeof(short))
                return () => default(short);
            
            if (nullableUnderlyingType == typeof(ushort))
                return () => default(ushort);
            
            if (nullableUnderlyingType == typeof(int))
                return () => default(int);
            
            if (nullableUnderlyingType == typeof(uint))
                return () => default(uint);
            
            if (nullableUnderlyingType == typeof(long))
                return () => default(long);
            
            if (nullableUnderlyingType == typeof(ulong))
                return () => default(ulong);
            
            if (nullableUnderlyingType == typeof(float))
                return () => default(float);
            
            if (nullableUnderlyingType == typeof(double))
                return () => default(double);
            
            if (nullableUnderlyingType == typeof(decimal))
                return () => default(decimal);

            return null;
        }
    }
}