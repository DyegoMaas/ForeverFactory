using System;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories.ReflectionTargets
{
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
}