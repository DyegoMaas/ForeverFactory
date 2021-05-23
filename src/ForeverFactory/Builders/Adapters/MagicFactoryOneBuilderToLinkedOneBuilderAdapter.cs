using System;
using System.Collections.Generic;
using ForeverFactory.Builders.Common;
using ForeverFactory.Transforms;

namespace ForeverFactory.Builders.Adapters
{
    internal class MagicFactoryOneBuilderToLinkedOneBuilderAdapter<T> : ILinkedBuilder<T>
        where T : class
    {
        private readonly IOneBuilder<T> _builder;
        private readonly TransformList<T> _defaultTransforms;
        private readonly Func<T> _customConstructor;

        public MagicFactoryOneBuilderToLinkedOneBuilderAdapter(
            MagicFactory<T> builder,
            TransformList<T> defaultTransforms,
            Func<T> customConstructor)
        {
            _builder = builder;
            _defaultTransforms = defaultTransforms;
            _customConstructor = customConstructor;
        }

        public ILinkedOneBuilder<T> PlusOne()
        {
            return new LinkedOneBuilder<T>(
                new SharedContext<T>(_defaultTransforms, _customConstructor),
                previous: this
            );
        }

        public IManyBuilder<T> Plus(int count)
        {
            return new ManyBuilder<T>(count, 
                new SharedContext<T>(_defaultTransforms, _customConstructor), 
                previous: null
            );
        }

        public IEnumerable<T> Build()
        {
            yield return _builder.Build();
        }
    }
}