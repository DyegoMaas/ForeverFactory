using System;
using System.Reflection;

namespace ForeverFactory.Generators.Transforms.Factories
{
    internal class FillWithEmptyStringTransformFactory : BaseRecursiveTransformFactory
    {
        protected override Func<object> GetBuildFunctionForProperty(PropertyInfo propertyInfo, int index)
        {
            if (propertyInfo.PropertyType == typeof(string)) 
                return () => string.Empty;

            return null;
        }
    }
}