using System;
using System.Linq;
using System.Reflection;

namespace ForeverFactory.Core.Transforms.Factories
{
    internal static class TransformFactory
    {
        public static Transform<T> FillWithEmptyValues<T>()
            where T : class
        {
            var setMember = new Func<T, object>(instance =>
            {
                FillPropertiesRecursively(instance, typeof(T));
                return instance;
            });
            return new ReflectedFuncTransform<T>(setMember);
        }

        private static void FillPropertiesRecursively(object instance, IReflect type)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                var buildFunction = GetBuildFunction(propertyInfo);
                if (buildFunction == null)
                    continue;

                var propertyValue = buildFunction.Invoke();
                propertyInfo.SetValue(instance, propertyValue);

                FillPropertiesRecursively(propertyValue, propertyInfo.PropertyType);
            }
        }

        private static Func<object> GetBuildFunction(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(string)) return () => string.Empty;

            var parameterlessConstructor = propertyInfo.PropertyType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null) return () => parameterlessConstructor.Invoke(new object[0]);

            return null;
        }
    }
}