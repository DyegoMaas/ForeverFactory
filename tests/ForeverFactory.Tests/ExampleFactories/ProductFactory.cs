﻿namespace ForeverFactory.Tests.ExampleFactories
{
    public class ProductFactory : MagicFactory<Product>
    {
        public ProductFactory()
        {
            UseConstructor(() => new Product("Nimbus 2000", "Brooms"));
            Set(x => x.Description = "You will fly");
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