using System;
using ForeverFactory.Behaviors;

namespace ForeverFactory
{
    public interface ICustomizeFactoryOptions<T>
        where T : class
    {
        ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor);
        ICustomizeFactoryOptions<T> SetDefaultBehavior(Behavior behavior);
        ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember);
        ICustomizeFactoryOptions<T> Do(Action<T> callback);
    }
}