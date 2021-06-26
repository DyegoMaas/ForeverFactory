using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Builders;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards;
using ForeverFactory.Core.Transforms.Guards.Specifications;

namespace ForeverFactory.Core
{
    internal class ObjectFactory<T> : IEnumerableBuilder<T>
        where T : class
    {
        private readonly List<NotGuardedTransform<T>> _defaultTransforms = new List<NotGuardedTransform<T>>();
        private readonly List<GeneratorNode<T>> _nodes = new List<GeneratorNode<T>>();

        public IEnumerable<T> Build()
        {
            return _nodes.SelectMany(generatorNode => generatorNode.ProduceInstances(_defaultTransforms));
        }

        public void AddDefaultTransform(Transform<T> transform)
        {
            _defaultTransforms.Add(new NotGuardedTransform<T>(transform));
        }

        public void AddTransform(Transform<T> transform, Func<GeneratorNode<T>, CanApplyTransformSpecification> guard)
        {
            var node = GetCurrentGeneratorNode();
            node.AddTransform(transform, guard(node));
        }

        private GeneratorNode<T> GetCurrentGeneratorNode()
        {
            return _nodes.Any()
                ? _nodes.Last()
                : null;
        }

        public void AddRootNode(GeneratorNode<T> generatorNode)
        {
            _nodes.Clear();
            AddNode(generatorNode);
        }

        public void AddNode(GeneratorNode<T> generatorNode)
        {
            _nodes.Add(generatorNode);
        }
    }
}