using System;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories.ReflectionTargets
{
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