﻿using System;
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

                FillPropertiesRecursively(propertyValue, propertyInfo.PropertyType, index: 0); // TODO test recursive behavior
            }
        }

        private static Func<object> GetBuildFunction(PropertyInfo propertyInfo, int index)
        {
            if (propertyInfo.PropertyType == typeof(string)) 
                return () => propertyInfo.Name + (index + 1);
            // TODO set numerical types

            var parameterlessConstructor = propertyInfo.PropertyType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null) 
                return () => parameterlessConstructor.Invoke(Array.Empty<object>());

            return null;
        }
    }
}