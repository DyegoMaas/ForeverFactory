using System;
using System.Linq;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithPropertyNameTransformFactory : ITranformFactory
    {
        public Transform<T> GetTransform<T>()
            where T : class
        {
            var setMember = new Func<T, int, object>((instance, index) =>
            {
                FillPropertiesRecursively(instance, typeof(T), index);
                return instance;
            });
            return new ReflectedFuncTransform<T>(setMember);
        }
        
        private static void FillPropertiesRecursively(object instance, IReflect type, int index)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                var buildFunction = GetBuildFunction(propertyInfo, index);
                if (buildFunction == null)
                    continue;

                var propertyValue = buildFunction.Invoke();
                propertyInfo.SetValue(instance, propertyValue);

                if (propertyInfo.PropertyType != typeof(string))
                {
                    FillPropertiesRecursively(propertyValue, propertyInfo.PropertyType, index);
                }
            }
        }

        private static Func<object> GetBuildFunction(PropertyInfo propertyInfo, int index)
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

            var parameterlessConstructor = propertyInfo.PropertyType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null) 
                return () => parameterlessConstructor.Invoke(Array.Empty<object>());

            return null;
        }
    }
}