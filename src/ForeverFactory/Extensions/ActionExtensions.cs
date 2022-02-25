using System;

namespace ForeverFactory
{
    internal static class ActionExtensions
    {
        public static Func<T, bool> WrapAsFunction<T>(this Action<T> callback)
        {
            bool Wrapper(T instance)
            {
                callback.Invoke(instance);
                return true;
            }

            return Wrapper;
        }
    }
}