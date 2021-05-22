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
    internal class ManyBuilder<T> : LinkedBaseBuilder<T>, IManyBuilder<T>
        where T : class
    {
        private readonly int _quantityToProduce;

        // private readonly Func<T> _customConstructor;
        // private readonly ILinkedBuilder<T> _previousBuilder;

        // public ManyBuilder(int quantityToProduce, TransformList<T> defaultTransforms, Func<T> customConstructor, ILinkedBuilder<T> previousBuilder = null)
        //     : base(previousBuilder)
        // {
        //     AddDefaultTransforms(defaultTransforms);
        //     // _customConstructor = customConstructor;
        //     SetCustomConstructor(customConstructor);
        //     _quantityToProduce = quantityToProduce;
        //     // _previousBuilder = previousBuilder;
        // }
        
        public ManyBuilder(int quantityToProduce, ISharedContext<T> sharedContext, ILinkedBuilder<T> previousBuilder = null)
            : base(previousBuilder)
        {
            AddDefaultTransforms(sharedContext.DefaultTransforms);
            SetCustomConstructor(sharedContext.CustomConstructor);
            _quantityToProduce = quantityToProduce;
            // _customConstructor = customConstructor;
            // _previousBuilder = previousBuilder;
        }

        private InstanceSetExecutionContext GetExecutionContext() => new InstanceSetExecutionContext(_quantityToProduce);

        /// <summary>
        /// Works within the active context
        /// </summary>
        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        /// <summary>
        /// Works within the active context
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            ValidateCount(count);
            
            AddTransform(new FuncTransform<T,TValue>(setMember, new ConditionToApplyFirst(count, GetExecutionContext())));
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
            AddTransform(new FuncTransform<T,TValue>(setMember, new ConditionToApplyLast(count, GetExecutionContext())));
            return this;
        }

        public ILinkedOneBuilder<T> PlusOne() // TODO test
        {
            return new LinkedOneBuilder<T>(DefaultTransforms, this);
        }

        /// <summary>
        /// Creates a new linked context
        /// </summary>
        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, 
                sharedContext: this,
                previousBuilder: this
            );
        }

        public ILinkedOneBuilder<T> PluOne()
        {
            return new LinkedOneBuilder<T>(DefaultTransforms, this);
        }

        public IEnumerable<T> Build()
        {
            foreach (var linkedInstance in Previous?.Build() ?? Enumerable.Empty<T>())
            {
                yield return linkedInstance;
            }

            for (var i = 0; i < _quantityToProduce; i++)
            {
                var instance = CreateInstance();
                foreach (var transform in GetTransformsToApply())
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