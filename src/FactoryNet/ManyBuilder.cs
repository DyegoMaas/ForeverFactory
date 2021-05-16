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

        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _count)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_count})"); // TODO test condition
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApply(Condition.First, count)));
            return this;
        }
        
        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _count)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_count})"); // TODO test condition
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApply(Condition.Last, count)));
            return this;
        }

        IEnumerable<T> IManyBuilder<T>.Build()
        {
            for (var i = 0; i < _count; i++)
            {
                T instance = new();
                foreach (var transform in _transforms)
                {
                    if (transform.ConditionToApply?.Condition == Condition.First)
                    {
                        if (i >= transform.ConditionToApply.Count)
                            continue;
                    }
                    
                    if (transform.ConditionToApply?.Condition == Condition.Last)
                    {
                        var firstToApply = _count - transform.ConditionToApply.Count;
                        if (i < firstToApply)
                            continue;
                    }
                    
                    transform.ApplyTo(instance);
                }
            
                yield return instance;
            }
        }
    }
}