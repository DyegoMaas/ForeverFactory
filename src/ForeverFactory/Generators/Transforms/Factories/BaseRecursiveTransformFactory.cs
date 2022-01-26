using System;
using System.Linq;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal abstract class BaseRecursiveTransformFactory : ITranformFactory
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
        
        private void FillPropertiesRecursively(object instance, IReflect type, int index)
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
                    FillPropertiesRecursively(propertyValue, propertyInfo.PropertyType, index);
            }
        }

        private Func<object> GetBuildFunction(PropertyInfo propertyInfo, int index)
        {
            var buildFunction = GetBuildFunctionForProperty(propertyInfo, index);
            if (buildFunction != null)
                return buildFunction;

            var parameterlessConstructor = propertyInfo.PropertyType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null) 
                return () => parameterlessConstructor.Invoke(Array.Empty<object>());

            return null;
        }

        protected abstract Func<object> GetBuildFunctionForProperty(PropertyInfo propertyInfo, int index);
    }
}