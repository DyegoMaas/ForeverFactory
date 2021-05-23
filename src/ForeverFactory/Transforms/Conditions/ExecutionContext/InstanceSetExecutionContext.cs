using ForeverFactory.Transforms.Conditions.ExecutionContext;

namespace ForeverFactory.ExecutionContext
{
    internal class InstanceSetExecutionContext : IExecutionContext
    {
        public int QuantityToProduce { get; }

        public InstanceSetExecutionContext(int quantityToProduce)
        {
            QuantityToProduce = quantityToProduce;
        }
    }
}