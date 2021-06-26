namespace ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories
{
    public class PersonFactory : MagicFactory<Person>
    {
        protected override void Customize(ICustomizeFactoryOptions<Person> customization)
        {
            customization
                .Set(x => x.FirstName = "Albert")
                .Set(x => x.LastName = "Einstein")
                .Set(x => x.Age = 56);
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}