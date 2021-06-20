using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ForeverFactory.Builders;
using ForeverFactory.Core;
using ForeverFactory.Core.FactoryBehaviors;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Core.Transforms.Guards.Specifications;

namespace ForeverFactory
{
    /// <summary>
    /// Allows the creation of a customizable factory.
    /// </summary>
    public static class MagicFactory
    {
        /// <summary>
        /// Creates a factory for building instances of type "T".
        /// </summary>
        /// <typeparam name="T">The factory will build instances of this type</typeparam>
        /// <returns>Factory of T</returns>
        public static ICustomizableFactory<T> For<T>() where T : class => new DynamicFactory<T>();
        
        private sealed class DynamicFactory<T> : MagicFactory<T>
            where T : class
        {
            protected override void Customize(ICustomizeFactoryOptions<T> customization)
            {
            }
        }
    }
    
    /// <summary>
    /// A customizable factory of objects of type "T". It can be extended with predefined configurations.
    /// </summary>
    /// <typeparam name="T">The type of objects that this factory will build.</typeparam>
    public abstract class MagicFactory<T> : ICustomizableFactory<T>, IManyBuilder<T>, ILinkedOneBuilder<T>
        where T : class
    {
        private readonly ObjectFactory<T> _objectFactory = new ObjectFactory<T>();
        private Func<T> _customConstructor;
        private GeneratorNode<T> _rootNode;
        private Behaviors _chosenBehavior = Behaviors.DotNotFill;

        protected MagicFactory()
        {
            SetRootNode(targetCount: 1);
            Customize(new CustomizeFactoryOptions<T>(this));
        }
        
        // TODO add test to ensure default transforms are not lost in the process
        private void SetRootNode(int targetCount)
        {
            _rootNode = new GeneratorNode<T>(targetCount, _customConstructor);
            _objectFactory.AddRootNode(_rootNode);
        }
        
        protected abstract void Customize(ICustomizeFactoryOptions<T> customization);

        public ICustomizableFactory<T> UsingConstructor(Func<T> customConstructor)
        {
            _customConstructor = customConstructor;
            _rootNode.OverrideCustomConstructor(newCustomConstructor: customConstructor);
            return this;
        }

        public ICustomizableFactory<T> WithBehavior(Behaviors chosenBehavior)
        {
            _chosenBehavior = chosenBehavior;

            switch (chosenBehavior)
            {
                case Behaviors.FillWithEmpty:
                    var transform = TransformFactory.GetFillWithEmptyFor<T>();
                    var node = _objectFactory.GetCurrentGeneratorNode();
                    node.AddTransformFirst(
                        transform: transform,
                        guard: new AlwaysApplyTransformSpecification()
                    );
                    break;
            }
            return this;
        }

        public IOneBuilder<T> With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        IManyBuilder<T> IManyBuilder<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        ILinkedOneBuilder<T> ILinkedOneBuilder<T>.With<TValue>(Func<T, TValue> setMember)
        {
            AddTransformThatAlwaysApply(setMember);
            return this;
        }

        public IManyBuilder<T> WithFirst<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToFirstNInstances(count, setMember);
            return this;
        }

        public IManyBuilder<T> WithLast<TValue>(int count, Func<T, TValue> setMember)
        {
            AddTransformThatAppliesToLastNInstances(count, setMember);
            return this;
        }

        private void AddTransformThatAlwaysApply<TValue>(Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: new AlwaysApplyTransformSpecification()
            );
        }

        private void AddTransformThatAppliesToFirstNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: new ApplyTransformToFirstInstancesSpecification(count)
            );
        }

        private void AddTransformThatAppliesToLastNInstances<TValue>(int count, Func<T, TValue> setMember)
        {
            var node = _objectFactory.GetCurrentGeneratorNode();
            node.AddTransform(
                transform: new FuncTransform<T, TValue>(setMember.Invoke),
                guard: new ApplyTransformToLastInstancesSpecification(count, node.TargetCount)
            );
        }

        public IManyBuilder<T> Many(int count)
        {
            SetRootNode(targetCount: count);
            return this;
        }
        
        public ILinkedOneBuilder<T> PlusOne()
        {
            var newNode = new GeneratorNode<T>(targetCount: 1, _customConstructor);
            _objectFactory.AddNode(newNode);
            
            return this;
        }

        public IManyBuilder<T> Plus(int count)
        {
            var newNode = new GeneratorNode<T>(targetCount: count, _customConstructor);
            _objectFactory.AddNode(newNode);

            return this;
        }
        
        public T Build()
        {
            return _objectFactory.Build().First();
        }

        IEnumerable<T> IBuilder<T>.Build()
        {
            return _objectFactory.Build();
        }
    }

    public static class TransformFactory
    {
        public static Transform<T> GetFillWithEmptyFor<T>() 
            where T : class
        {
            var setMember = new Func<T, object>(instance =>
            {
                FillPropertiesRecursively(instance, typeof(T));
                return instance;
            });
            return new ReflectedFuncTransform<T>(setMember);
        }

        private static void FillPropertiesRecursively(object instance, IReflect type)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                var buildFunction = GetBuildFunction(propertyInfo);
                if (buildFunction == null) 
                    continue;
                
                var propertyValue = buildFunction.Invoke();
                propertyInfo.SetValue(instance, propertyValue);
                
                FillPropertiesRecursively(propertyValue, propertyInfo.PropertyType);
            }
        }

        private static Func<object> GetBuildFunction(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(string))
            {
                return () => string.Empty;
            }
            
            var parameterlessConstructor = propertyInfo.PropertyType
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (parameterlessConstructor != null)
            {
                return () => parameterlessConstructor.Invoke(new object[0]);
            }

            return null;
        }
    }

    public class CustomizeFactoryOptions<T> : ICustomizeFactoryOptions<T>
        where T : class
    {
        private readonly MagicFactory<T> _magicFactory;

        public CustomizeFactoryOptions(MagicFactory<T> magicFactory)
        {
            _magicFactory = magicFactory;
        }

        public ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor)
        {
            _magicFactory.UsingConstructor(customConstructor);
            return this;
        }

        public ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember)
        {
            _magicFactory.With(setMember);
            return this;
        }

        public ICustomizeFactoryOptions<T> SetPropertyFillBehavior(Behaviors behavior)
        {
            _magicFactory.WithBehavior(behavior);
            return this;
        }
    }

     public interface ICustomizeFactoryOptions<T>
        where T : class
     {
         ICustomizeFactoryOptions<T> UseConstructor(Func<T> customConstructor);
         ICustomizeFactoryOptions<T> Set<TValue>(Func<T, TValue> setMember);
         ICustomizeFactoryOptions<T> SetPropertyFillBehavior(Behaviors behavior);
     }
}