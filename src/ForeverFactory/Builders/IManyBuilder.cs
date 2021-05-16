﻿using System;
using System.Collections.Generic;

namespace ForeverFactory.Builders
{
    public interface IManyBuilder<out T>
    {
        IManyBuilder<T> With<TValue>(Func<T, TValue> setMember);
        IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember);
        IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember);
        IManyBuilder<T> Plus(int count);
        IEnumerable<T> Build();
    }
}