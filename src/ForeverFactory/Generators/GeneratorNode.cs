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
        private Func<T> _customConstructor;

        public GeneratorNode(
            int targetCount = 1,
            Func<T> customConstructor = null)
        {
            TargetCount = targetCount;
            _customConstructor = customConstructor;
        }

        public int TargetCount { get; }

        public void AddTransform(Transform<T> transform, CanApplyTransformSpecification guard = null)
        {
            guard = guard ?? new AlwaysApplyTransformSpecification();
            _transformsToApply.Add(new GuardedTransform<T>(transform, guard));
        }

        public void OverrideCustomConstructor(Func<T> newCustomConstructor)
        {
            _customConstructor = newCustomConstructor;
        }

        public IEnumerable<T> ProduceInstances(IEnumerable<NotGuardedTransform<T>> defaultTransforms = null)
        {
            for (var index = 0; index < TargetCount; index++)
            {
                var instance = CreateInstance();

                var defaultTransformsToApply = defaultTransforms ?? Enumerable.Empty<GuardedTransform<T>>();
                var transformsToApply = defaultTransformsToApply.Union(_transformsToApply);
                ApplyTransformsToInstance(transformsToApply, instance, index);

                yield return instance;
            }
        }

        private T CreateInstance()
        {
            return _customConstructor != null
                ? _customConstructor.Invoke()
                : Activator.CreateInstance<T>();
        }

        private void ApplyTransformsToInstance(IEnumerable<GuardedTransform<T>> guardedTransforms, T instance,
            int instanceIndex)
        {
            foreach (var guardedTransform in guardedTransforms)
                if (guardedTransform.Guard.CanApply(instanceIndex))
                    guardedTransform.Transform.ApplyTo(instance, instanceIndex);
        }
    }
}