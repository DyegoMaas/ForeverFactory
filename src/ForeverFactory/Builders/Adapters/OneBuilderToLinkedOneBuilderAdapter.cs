using System.Collections.Generic;

namespace ForeverFactory.Builders.Adapters
{
    internal class OneBuilderToLinkedOneBuilderAdapter<T> : ILinkedBuilder<T>
        where T : class
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