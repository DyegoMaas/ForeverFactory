using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Builders.Common;
using ForeverFactory.ExecutionContext;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class ManyBuilder<T> : LinkedBaseBuilder<T>, IManyBuilder<T>
        where T : class
    {
        private readonly int _quantityToProduce;
        
        public ManyBuilder(int quantityToProduce, ISharedContext<T> sharedContext, ILinkedBuilder<T> previous = null)
            : base(sharedContext, previous)
        {
            _quantityToProduce = quantityToProduce;
        }

        private InstanceSetExecutionContext GetExecutionContext() => new InstanceSetExecutionContext(_quantityToProduce);

        public IManyBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

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
            return new LinkedOneBuilder<T>(SharedContext, previous: this);
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, SharedContext, previous: this);
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