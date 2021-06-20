namespace ForeverFactory.Tests.ExampleFactories
{
    public class ProductFactory : MagicFactory<Product>
    {
        public ProductFactory()
        {
            UseConstructor(() => new Product("Nimbus 2000", "Brooms"));
            Set(x => x.Description = "The best flight await you!");
        }
    }
    
    public class ProductFactory2 : MagicFactory2<Product>
    {
        public ProductFactory2()
        {
            UseConstructor(() => new Product("Nimbus 2000", "Brooms"));
            Set(x => x.Description = "The best flight await you!");
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