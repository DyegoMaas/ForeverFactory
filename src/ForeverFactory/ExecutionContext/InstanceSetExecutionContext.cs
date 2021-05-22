namespace ForeverFactory.ExecutionContext
{
    internal class InstanceSetExecutionContext : IExecutionContext // TODO rename
    {
        public int QuantityToProduce { get; }

        public InstanceSetExecutionContext(int quantityToProduce)
        {
            QuantityToProduce = quantityToProduce;
        }
    }
}