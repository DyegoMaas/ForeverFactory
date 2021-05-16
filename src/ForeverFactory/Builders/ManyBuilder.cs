using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.ExecutionContext;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    /*
     * Assumptions:
     * - transformations are applied in same order they are declared: this makes the system deterministic and
     * consequently more understandable
     */
    internal class ManyBuilder<T> : IManyBuilder<T>
        where T : class
    {
        private readonly List<Transform<T>> _defaultTransforms;
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _quantityToProduce;

        private readonly Func<T> _customConstructor;
        private readonly ManyBuilder<T> _previousBuilder;

        public ManyBuilder(int quantityToProduce, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor)
        {
            _defaultTransforms = defaultTransforms.ToList();
            _transforms.AddRange(_defaultTransforms);
            _customConstructor = customConstructor;
            _quantityToProduce = quantityToProduce;
        }
        
        private ManyBuilder(int quantityToProduce, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor, ManyBuilder<T> previousBuilder)
            : this(quantityToProduce, defaultTransforms, customConstructor)
        {
            _previousBuilder = previousBuilder;
        }

        private InstanceSetExecutionContext GetExecutionContext() => new(_quantityToProduce);

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
            
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyFirst(count, GetExecutionContext())));
            return this;
        }

        private void ValidateCount(int count)
        {
            if (count > _quantityToProduce)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_quantityToProduce})");
            }
        }

        /// <summary>
        /// Works within the active context
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            if (count > _quantityToProduce)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_quantityToProduce})");
            }
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyLast(count, GetExecutionContext())));
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

            for (var i = 0; i < _quantityToProduce; i++)
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