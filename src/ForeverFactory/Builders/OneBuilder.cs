using System;
using System.Collections.Generic;
using ForeverFactory.Builders.Common;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class OneBuilder<T> : BaseBuilder<T>, IOneBuilder<T>
        where T : class
    {
        public OneBuilder(ISharedContext<T> sharedContext) : base(sharedContext)
        {
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T, TValue>(setMember, new NoConditionToApply()));
            return this;
        }
        
        public IOneBuilder<T> With(Transform<T> setMember)
        {
            AddTransform(setMember);
            return this;
        }

        public T Build()
        {
            var instance = CreateInstance();
            foreach (var transform in GetTransformsToApply())
            {
                transform.ApplyTo(instance);
            }
            
            return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(SharedContext, new OneBuilderToLinkedOneBuilderAdapter(this));
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, SharedContext,    
                previous: new OneBuilderToLinkedOneBuilderAdapter(this))
            ; // TODO review custom constructor (difference between one set in custom factory and with UsingConstructor 
        }

        private class OneBuilderToLinkedOneBuilderAdapter : ILinkedBuilder<T>
        {
            private readonly OneBuilder<T> _builder;

            public OneBuilderToLinkedOneBuilderAdapter(OneBuilder<T> builder)
            {
                _builder = builder;
            }

            public ILinkedOneBuilder<T> PlusOne()
            {
                return new LinkedOneBuilder<T>(_builder.SharedContext, this);
            }

            public IManyBuilder<T> Plus(int count)
            {
                return new ManyBuilder<T>(count,
                    sharedContext: _builder.SharedContext,
                    previous: null
                );
            }

            public IEnumerable<T> Build()
            {
                yield return _builder.Build();
            }
        }
    }
}