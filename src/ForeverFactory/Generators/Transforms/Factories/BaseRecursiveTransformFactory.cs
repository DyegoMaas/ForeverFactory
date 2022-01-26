using System;
using System.Linq;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal abstract class BaseRecursiveTransformFactory : ITranformFactory
    {
        private readonly RecursiveTransformFactoryOptions _options;

        protected BaseRecursiveTransformFactory(RecursiveTransformFactoryOptions options = null)
        {
            _options = options ?? new RecursiveTransformFactoryOptions();
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

            if (_options.EnableRecursiveInstantiation is false)
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
            return _options.EnableRecursiveInstantiation && targetInfo.TargetType != typeof(string);
        }
    }
    
    internal abstract class TargetInfo
    {
        public abstract Type TargetType { get; }
        public abstract string Name { get; }
            
        public abstract void SetValue(object instance, object value);
    };

    internal sealed class PropertyTargetInfo : TargetInfo
    {
        private readonly PropertyInfo _propertyInfo;

        public override Type TargetType => _propertyInfo.PropertyType;
        public override string Name => _propertyInfo.Name;
            

        public PropertyTargetInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public override void SetValue(object instance, object value)
        {
            _propertyInfo.SetValue(instance, value);
        }
    }
        
    internal sealed class FieldTargetInfo : TargetInfo
    {
        private readonly FieldInfo _fieldInfo;
            
        public override Type TargetType => _fieldInfo.FieldType;
        public override string Name => _fieldInfo.Name;

        public FieldTargetInfo(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public override void SetValue(object instance, object value)
        {
            _fieldInfo.SetValue(instance, value);
        }
    }
}