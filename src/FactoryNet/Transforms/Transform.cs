using FactoryNet.Transforms.Conditions;

namespace FactoryNet.Transforms
{
    public abstract class Transform<T>
    {
        public ConditionToApply ConditionToApply { get; }

        protected Transform(ConditionToApply conditionToApply = null)
        {
            ConditionToApply = conditionToApply;
        }

        public abstract void ApplyTo(T instance);
    }
}