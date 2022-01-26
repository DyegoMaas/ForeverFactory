using System;
using ForeverFactory.Generators.Transforms.Factories.ReflectionTargets;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithEmptyStringTransformFactory : BaseRecursiveTransformFactory
    {
        public FillWithEmptyStringTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(TargetInfo targetInfo, int index)
        {
            if (targetInfo.TargetType == typeof(string)) 
                return () => string.Empty;

            return null;
        }
    }
}