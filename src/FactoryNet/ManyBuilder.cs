using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryNet
{
    public class ManyBuilder<T> : IManyBuilder<T>
        where T : new()
    {
        private readonly List<Transform<T>> _defaultTransforms;
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _count;
        
        private readonly ManyBuilder<T> _previousBuilder;

        public ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms)
        {
            _defaultTransforms = defaultTransforms.ToList();
            _transforms.AddRange(_defaultTransforms);
            _count = count;
        }
        
        private ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms, ManyBuilder<T> previousBuilder)
            : this(count, defaultTransforms)
        {
            _previousBuilder = previousBuilder;
        }

        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _count)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_count})");
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyFirst(count, setSize: _count)));
            return this;
        }
        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _count)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_count})");
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyLast(count, setSize: _count)));
            return this;
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, _defaultTransforms, previousBuilder: this);
        }

        public IEnumerable<T> Build()
        {
            foreach (var instance in _previousBuilder?.Build() ?? Enumerable.Empty<T>())
            {
                yield return instance;
            }

            for (var i = 0; i < _count; i++)
            {
                T instance = new();
                foreach (var transform in _transforms)
                {
                    if (transform.ConditionToApply.CanApplyFor(index: i) is false)
                        continue;
                    
                    transform.ApplyTo(instance);
                }
            
                yield return instance;
            }
        }
    }
}