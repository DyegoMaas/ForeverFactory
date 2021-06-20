using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Builders;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards;

namespace ForeverFactory.Core
{
    internal class ObjectFactory<T> : IBuilder<T> 
        where T : class
    {
        private readonly LinkedList<GeneratorNode<T>> _nodes = new LinkedList<GeneratorNode<T>>();
        private readonly List<NotGuardedTransform<T>> _defaultTransforms = new List<NotGuardedTransform<T>>();
        
        public void AddDefaultTransform(Transform<T> transform)
        {
            _defaultTransforms.Add(new NotGuardedTransform<T>(transform));
        }

        public void AddRootNode(GeneratorNode<T> generatorNode)
        {
            _nodes.Clear();
            AddNode(generatorNode);
        }

        public void AddNode(GeneratorNode<T> generatorNode)
        {
            _nodes.AddLast(generatorNode);
        }

        public IEnumerable<T> Build()
        {
            return _nodes.SelectMany(generatorNode => generatorNode.ProduceInstances(_defaultTransforms));
        }

        public GeneratorNode<T> GetCurrentGeneratorNode()
        {
            return _nodes.Any()
                ? _nodes.Last.Value
                : null;
        }

        public bool HasNodes()
        {
            return GetCurrentGeneratorNode() != null;
        }
    }
}