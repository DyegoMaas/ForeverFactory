using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Generators.Transforms.Guards;
using ForeverFactory.Generators.Transforms.Guards.Specifications;

namespace ForeverFactory.Generators
{
    internal class GeneratorNode<T>
        where T : class
    {
        private readonly List<GuardedTransform<T>> _transformsToApply = new List<GuardedTransform<T>>();

        public GeneratorNode(int instanceCount = 1)
        {
            InstanceCount = instanceCount;
        }

        public int InstanceCount { get; }

        public void AddTransform(Transform<T> transform, CanApplyTransformSpecification guard = null)
        {
            guard = guard ?? new AlwaysApplyTransformSpecification();
            _transformsToApply.Add(new GuardedTransform<T>(transform, guard));
        }

        public IEnumerable<T> GenerateInstances(IEnumerable<NotGuardedTransform<T>> defaultTransforms = null, Func<T> customConstructor = null)
        {
            for (var index = 0; index < InstanceCount; index++)
            {
                var instance = CreateInstance(customConstructor);

                var defaultTransformsToApply = defaultTransforms ?? Enumerable.Empty<GuardedTransform<T>>();
                var transformsToApply = defaultTransformsToApply
                    .Union(_transformsToApply);
                ApplyTransformsToInstance(transformsToApply, instance, index);

                yield return instance;
            }
        }

        private static T CreateInstance(Func<T> customConstructor = null)
        {
            return customConstructor != null
                ? customConstructor.Invoke()
                : Activator.CreateInstance<T>();
        }

        private static void ApplyTransformsToInstance(IEnumerable<GuardedTransform<T>> guardedTransforms, T instance,
            int instanceIndex)
        {
            foreach (var guardedTransform in guardedTransforms)
                if (guardedTransform.Guard.CanApply(instanceIndex))
                    guardedTransform.Transform.ApplyTo(instance, instanceIndex);
        }
    }
}