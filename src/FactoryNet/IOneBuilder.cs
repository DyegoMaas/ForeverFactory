using System;

namespace FactoryNet
{
    public interface IOneBuilder<out T>
    {
        IOneBuilder<T> With<TValue>(Func<T, TValue> setMember);
        T Build();
    }
}