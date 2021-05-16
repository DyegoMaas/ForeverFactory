using System;
using System.Collections.Generic;

namespace FactoryNet
{
    public class ManyBuilder<T> : IManyBuilder<T>
        where T : new()
    {
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _count;

        public ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms)
        {
            _count = count;
            _transforms.AddRange(defaultTransforms);
        }

        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T,TValue>(setMember));
            return this;
        }

        IEnumerable<T> IManyBuilder<T>.Build()
        {
            for (var i = 0; i < _count; i++)
            {
                T instance = new();
                foreach (var transform in _transforms)
                {
                    transform.ApplyTo(instance);
                }
            
                yield return instance;
            }
        }
    }
}