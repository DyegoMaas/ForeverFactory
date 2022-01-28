using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.FluentInterfaces;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Guards;
using ForeverFactory.Generators.Transforms.Guards.Specifications;

namespace ForeverFactory.Generators
{
    internal class ObjectFactory<T> : IBuildMany<T>
        where T : class
    {
        private readonly IObjectFactoryOptions<T> _options;
        private readonly List<GeneratorNode<T>> _generatorNodes = new List<GeneratorNode<T>>();

        public ObjectFactory(IObjectFactoryOptions<T> options)
        {
            _options = options;
        }

        public IEnumerable<T> Build()
        {
            var behaviorTransforms = _options.SelectedBehavior.GetTransforms<T>();
            var optionsTransforms = _options.Transforms;
            var notGuardedTransforms = behaviorTransforms
                .Union(optionsTransforms)
                .Select(transform => new NotGuardedTransform<T>(transform));
            
            return _generatorNodes.SelectMany(node => node.GenerateInstances(notGuardedTransforms, _options.CustomConstructor));
        }

        public void AddTransform(Transform<T> transform, Func<GeneratorNode<T>, CanApplyTransformSpecification> guard)
        {
            var node = GetCurrentGeneratorNode();
            node.AddTransform(transform, guard(node));
        }

        private GeneratorNode<T> GetCurrentGeneratorNode()
        {
            return _generatorNodes.LastOrDefault();
        }

        public void AddRootNode(GeneratorNode<T> generatorNode)
        {
            _generatorNodes.Clear();
            AddNode(generatorNode);
        }

        public void AddNode(GeneratorNode<T> generatorNode)
        {
            _generatorNodes.Add(generatorNode);
        }
    }
}