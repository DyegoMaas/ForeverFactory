using System;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithEmptyStringTransformFactory : BaseRecursiveTransformFactory
    {
        public FillWithEmptyStringTransformFactory(RecursiveTransformFactoryOptions options = null) 
            : base(options)
        {
        }

        protected override Func<object> GetBuildFunctionForSpecializedProperty(PropertyInfo propertyInfo, int index)
        {
            if (propertyInfo.PropertyType == typeof(string)) 
                return () => string.Empty;

            return null;
        }
    }
}