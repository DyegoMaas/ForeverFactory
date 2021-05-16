using System;
using System.Collections.Generic;
using System.Linq;
using FactoryNet.Transforms;
using FactoryNet.Transforms.Conditions;

namespace FactoryNet.Builders
{
    /*
     * Assumptions:
     * - transformations are applied in same order they are declared: this makes the system deterministic and
     * consequently more understandable
     */
    public class ManyBuilder<T> : IManyBuilder<T>
        where T : class
    {
        private readonly List<Transform<T>> _defaultTransforms;
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _setSize;

        private readonly Func<T> _customConstructor;
        private readonly ManyBuilder<T> _previousBuilder;

        public ManyBuilder(int setSize, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor)
        {
            _defaultTransforms = defaultTransforms.ToList();
            _transforms.AddRange(_defaultTransforms);
            _customConstructor = customConstructor;
            _setSize = setSize;
        }
        
        private ManyBuilder(int setSize, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor, ManyBuilder<T> previousBuilder)
            : this(setSize, defaultTransforms, customConstructor)
        {
            _previousBuilder = previousBuilder;
        }

        /// <summary>
        /// Works within the active context
        /// </summary>
        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        /// <summary>
        /// Works within the active context
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            ValidateCount(count);
            
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyFirst(count, setSize: _setSize)));
            return this;
        }

        private void ValidateCount(int count)
        {
            if (count > _setSize)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_setSize})");
            }
        }

        /// <summary>
        /// Works within the active context
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _setSize)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_setSize})");
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyLast(count, setSize: _setSize)));
            return this;
        }

        /// <summary>
        /// Creates a new linked context
        /// </summary>
        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, _defaultTransforms, _customConstructor, previousBuilder: this);
        }

        public IEnumerable<T> Build()
        {
            foreach (var instance in _previousBuilder?.Build() ?? Enumerable.Empty<T>())
            {
                yield return instance;
            }

            for (var i = 0; i < _setSize; i++)
            {
                var instance = _customConstructor?.Invoke() ?? Activator.CreateInstance<T>();
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