using System;
using System.Collections.Generic;
using System.Linq;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards;
using ForeverFactory.Core.Transforms.Guards.Specifications;

namespace ForeverFactory.Core
{
    internal class GeneratorNode<T>
        where T : class
    {
        private readonly List<GuardedTransform<T>> _transformsToApply = new List<GuardedTransform<T>>();

        public GeneratorNode(
            int targetCount = 1, 
            Func<T> customConstructor = null)
        {
            TargetCount = targetCount;
            CustomConstructor = customConstructor;
        }

        public int TargetCount { get; }
        public Func<T> CustomConstructor { get; set; }

        public void AddTransform(Transform<T> transform, ApplyTransformGuardSpecification guard = null)
        {
            guard = guard ?? new AlwaysApplyTransformGuardSpecification();
            _transformsToApply.Add(new GuardedTransform<T>(transform, guard));
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
            return CustomConstructor != null 
                ? CustomConstructor.Invoke() 
                : Activator.CreateInstance<T>();
        }

        private void ApplyTransformsToInstance(IEnumerable<GuardedTransform<T>> guardedTransforms, T instance, int instanceIndex)
        {
            foreach (var guardedTransform in guardedTransforms)
            {
                if (guardedTransform.Guard.CanApply(instanceIndex))
                {
                    guardedTransform.Transform.ApplyTo(instance);
                }
            }
        }
    }
}