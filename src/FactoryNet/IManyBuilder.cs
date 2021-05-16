using System;
using System.Collections.Generic;

namespace FactoryNet
{
    public interface IManyBuilder<out T>
    {
        IManyBuilder<T> With<TValue>(Func<T, TValue> setMember);
        IEnumerable<T> Build();
    }
}