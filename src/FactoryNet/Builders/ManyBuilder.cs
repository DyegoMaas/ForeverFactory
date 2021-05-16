﻿using System;
using System.Collections.Generic;
using System.Linq;
using FactoryNet.Transforms;
using FactoryNet.Transforms.Conditions;

namespace FactoryNet.Builders
{
    public class ManyBuilder<T> : IManyBuilder<T>
        where T : class
    {
        private readonly List<Transform<T>> _defaultTransforms;
        private readonly List<Transform<T>> _transforms = new();
        private readonly int _count;

        private readonly Func<T> _customConstructor;
        private readonly ManyBuilder<T> _previousBuilder;

        public ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor)
        {
            _defaultTransforms = defaultTransforms.ToList();
            _transforms.AddRange(_defaultTransforms);
            _customConstructor = customConstructor;
            _count = count;
        }
        
        private ManyBuilder(int count, IEnumerable<Transform<T>> defaultTransforms, Func<T> customConstructor, ManyBuilder<T> previousBuilder)
            : this(count, defaultTransforms, customConstructor)
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
            ValidateCount(count);
            
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyFirst(count, setSize: _count)));
            return this;
        }

        public IManyBuilder<T> WithNext<TValue>(int count, Func<T, TValue> setMember)
        {
            ValidateCount(count);

            var numberOfInstancesToSkip = _transforms
                .Select(x => x.ConditionToApply)
                .Where(x => x is ConditionToApplyFirst or ConditionToApplyBetween)
                .Sum(x => x.CountToApply);
            var startingIndex = numberOfInstancesToSkip;
            
            _transforms.Add(new FuncTransform<T,TValue>(setMember, new ConditionToApplyBetween(count, startingFromIndex: startingIndex, setSize: _count)));
            return this;
        }

        private void ValidateCount(int count)
        {
            if (count > _count)
            {
                throw new ArgumentOutOfRangeException("count", count,
                    $"Count should be less or equal to the set size ({_count})");
            }
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
            return new ManyBuilder<T>(count, _defaultTransforms, _customConstructor, previousBuilder: this);
        }

        public IEnumerable<T> Build()
        {
            foreach (var instance in _previousBuilder?.Build() ?? Enumerable.Empty<T>())
            {
                yield return instance;
            }

            for (var i = 0; i < _count; i++)
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