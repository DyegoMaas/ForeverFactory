using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Transforms;
using ForeverFactory.Transforms.Conditions;

namespace ForeverFactory.Builders
{
    internal class LinkedOneBuilder<T> : LinkedBaseBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        public LinkedOneBuilder(TransformList<T> defaultTransforms, ILinkedBuilder<T> previous)
            : base (previous)
        {
            AddDefaultTransforms(defaultTransforms);
        }

        public ILinkedOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransform(new FuncTransform<T,TValue>(setMember, new NoConditionToApply()));
            return this;
        }

        public IEnumerable<T> Build()
        {
            foreach (var linkedInstance in Previous?.Build() ?? Enumerable.Empty<T>())
            {
                yield return linkedInstance;
            }

            var instance = CreateInstance();
            foreach (var transform in GetTransformsToApply())
            {
                transform.ApplyTo(instance);
            }
            
            yield return instance;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(DefaultTransforms, this);
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, 
                sharedContext: this,
                previousBuilder: this
            );
        }
    }
}