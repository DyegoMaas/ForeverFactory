namespace ForeverFactory.Transforms.Conditions.ExecutionContext
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