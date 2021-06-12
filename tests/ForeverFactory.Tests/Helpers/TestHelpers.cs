using System;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Tests.Helpers
{
    internal static class TestHelpers 
    {
        public static Transform<T> FuncTransform<T, TValue>(Func<T, TValue> setMember)
        {
            return new FuncTransform<T, TValue>(setMember, new NoConditionToApply());
        } 
    }
}