using System;
using ForeverFactory.Builders.Adapters;
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
            AddTransform(new FuncTransform<T, TValue>(setMember, Conditions.NoConditions()));
            return this;
        }
        
        public IOneBuilder<T> With(Transform<T> setMember) // TODO test better
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
            return new LinkedOneBuilder<T>(SharedContext, new OneBuilderToLinkedOneBuilderAdapter<T>(this));
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new LinkedManyBuilder<T>(count, SharedContext,    
                previous: new OneBuilderToLinkedOneBuilderAdapter<T>(this))
            ; // TODO review custom constructor (difference between one set in custom factory and with UsingConstructor 
        }
    }
}