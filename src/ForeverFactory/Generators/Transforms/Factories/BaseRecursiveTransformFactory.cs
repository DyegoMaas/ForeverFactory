using System;
using System.Linq;
using System.Reflection;
using ForeverFactory.Generators.Transforms.Factories.ReflectionTargets;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal abstract class BaseRecursiveTransformFactory : ITranformFactory
    {
        protected readonly RecursiveTransformFactoryOptions Options;

        protected BaseRecursiveTransformFactory(RecursiveTransformFactoryOptions options = null)
        {
            Options = options ?? new RecursiveTransformFactoryOptions();
        }

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
            var propertyInfos = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(propertyInfo => new PropertyTargetInfo(propertyInfo));
            var fieldInfos = type
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(fieldInfo => new FieldTargetInfo(fieldInfo));
            var targetInfos = propertyInfos.Cast<TargetInfo>().Union(fieldInfos);
            foreach (var targetInfo in targetInfos)
            {
                var buildFunction = GetBuildFunction(targetInfo, index);
                if (buildFunction == null)
                    continue;

                var propertyValue = buildFunction.Invoke();
                targetInfo.SetValue(instance, propertyValue);

                if (CanApplyRecursion(targetInfo))
                    FillPropertiesRecursively(propertyValue, targetInfo.TargetType, index);
            }
        }

        private Func<object> GetBuildFunction(TargetInfo targetInfo, int index)
        {
            var buildFunction = GetBuildFunctionForSpecializedProperty(targetInfo, index);
            if (buildFunction != null)
                return buildFunction;

            if (Options.EnableRecursiveInstantiation is false)
                return null;

            var parameterlessConstructor = targetInfo.TargetType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null) 
                return () => parameterlessConstructor.Invoke(Array.Empty<object>());

            return null;
        }

        protected abstract Func<object> GetBuildFunctionForSpecializedProperty(TargetInfo targetInfo, int index);

        private bool CanApplyRecursion(TargetInfo targetInfo)
        {
            var isNullable = Nullable.GetUnderlyingType(targetInfo.TargetType) != null;
            var isExcludedType = targetInfo.TargetType == typeof(string) ||
                                 targetInfo.TargetType == typeof(DateTime); 
            return Options.EnableRecursiveInstantiation && !isExcludedType && !isNullable;
        }
    }
}