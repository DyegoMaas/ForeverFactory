namespace ForeverFactory.Tests.CustomizedFactories.ExampleFactories
{
    public class ProductFactory : MagicFactory<Product>
    {
        protected override void Customize(ICustomizeFactoryOptions<Product> customization)
        {
            customization
                .UseConstructor(() => new Product("Nimbus 2000", "Brooms"))
                .Set(x => x.Description = "The best flight await you!");
        }
    }

    public class Product
    {
        public string Name { get; }
        public string Category { get; }
        public string Description { get; set; }

        public Product(string name, string category)
        {
            Name = name;
            Category = category;
        }
    }
}