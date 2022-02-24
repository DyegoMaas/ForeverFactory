****# Forever Factory - Build test objects quickly

![License](https://img.shields.io/github/license/DyegoMaas/ForeverFactory.svg)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/DyegoMaas/ForeverFactory/build-and-test?label=tests)
[![codecov](https://codecov.io/gh/DyegoMaas/ForeverFactory/branch/main/graph/badge.svg?token=IPBG3BP2D8)](https://codecov.io/gh/DyegoMaas/ForeverFactory)
![Nuget](https://img.shields.io/nuget/v/ForeverFactory)
![Nuget](https://img.shields.io/nuget/dt/ForeverFactory)

**Forever Factory** helps you to easily build lots of custom objects. Some situations where it shines and is most helpful are:

- Creating test objects for any kind of automated tests, like unit, functional or acceptance tests
- Creating objects to return from mocked services
- Creating test data for when you are developing or testing new applications

**With ForeverFactory, building a new object can be as simple as `MagicFactory.For<Person>().Build()`.**

## How to use?

### Installing ForeverFactory

You may install ForeverFactory with NuGet:

`Install-Package ForeverFactory`

Or via the .NET Core command line interface:

`dotnet add package ForeverFactory`

Either commands, from Package Manager Console or .NET Core CLI, will download and install ForeverFactory.

### Building a single object

Let's assume we have a class named Person:

```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
```

We can build a new instance of this class as follows:

```csharp
var person = MagicFactory.For<Person>().Build();
```

And we can customize each property:

```csharp
var vitorHugo = MagicFactory.For<Person>()
    .With(x => x.FirstName = "Vitor")
    .With(x => x.LastName = "Hugo")
    .With(x => x.Email = $"{x.FirstName}.{LastName}@gmail.com".ToLower())
    .Build();
```

### Building multiple objects

We can build a defined number of instances of this class as follows:

```csharp
var hundredPeople = MagicFactory.For<Person>().Many(100).Build();
```

We can even customize a subset of the instances with custom properties:

```csharp
var hundredPeople = MagicFactory.For<Person>()
    .Many(100)
    .With(x => x.Age = 100) // applies to all instances
    .WithFirst(10, x => x.FirstName = "Isaac") // applies only to the first 10 instances
    .WithLast(20, x => x.LastName = "Clarke") // applies only to the last 20 instances
    .Build();
```

Additionally, we can create multiple sets of objects:

```csharp
var people = MagicFactory.For<Person>()
    .Many(10).With(x => x.Age = 5)
    .Plus(20).With(x => x.Age = 60)
    .PlusOne().With(x => x.Age = 100)
    .Build(); // creates 10 persons with age 5, 20 with age 60, and 1 with age 100
```

### Custom Factories

You can maximize reuse of common customization by creating your own factory, with predefined rules, like this:

```csharp
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
```

With this factory, you can generate many objects like this:

```csharp
var person = new PersonFactory().Build();

person.FirstName.Should().Be("Albert");
person.LastName.Should().Be("Einstein");
person.Age.Should().Be(56);
```

This feature is heavily inspired by a Python project named [factory_boy](https://github.com/FactoryBoy/factory_boy).

### Custom constructors

If your class has a constructor with parameters, like the one below, you'll have to configure it:

```csharp
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
```

If you are using a *custom factory*, you can set the constructor function there:

```csharp
public class ProductFactory : MagicFactory<Product>
{
    protected override void Customize(ICustomizeFactoryOptions<Product> customization)
    {
        customization
            .UseConstructor(() => new Product("Nimbus 2000", "Brooms"))
            .Set(x => x.Description = "Top of the line flying broom");
    }
}
```

Otherwise, you can just set it when building an object:
```csharp
var product = MagicFactory.For<Product>()
    .UsingConstructor(() => new Product("Nimbus 2000", "Brooms"))
    .With(x => x.Description = "Top of the line flying broom")
    .Build();
```

### Behaviors

You can change how **ForeverFactory** behaves when creating objects for you.

```csharp
var product = MagicFactory.For<Product>()
    .WithBehavior(new FillWithEmptyValuesBehavior())
    .Build();

// or 

private class ProductFactory : MagicFactory<Product>
{
    protected override void Customize(ICustomizeFactoryOptions<Product> customization)
    {
        customization
            .SetDefaultBehavior(new FillWithEmptyValuesBehavior());
    }
}
```

#### DoNotFillBehavior

By default, it will not fill anything, and it is up to you to fill any properties you need.

#### FillWithSequentialValuesBehavior

With this behavior, ForeverFactory will recursively initialize every property it can with sequential values. This is similar to the default behavior of NBuilder:

```csharp
var customers = MagicFactory.For<Customer>()
    .WithBehavior(new FillWithSequentialValuesBehavior())
    .Many(2)
    .Build();
   
customers[0].Name.Should().Be("Name1");
customers[0].Age.Should().Be(1);
customers[0].Address.ZipCode.Should().Be("ZipCode1");
customers[1].Name.Should().Be("Name2");
customers[1].Age.Should().Be(2);
customers[1].Address.ZipCode.Should().Be("ZipCode2");

public class Customer
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; } 
}

public class Address
{
    public string ZipCode { get; set; }
}
```

#### FillWithEmptyValuesBehavior

With this behavior, ForeverFactory will recursively initialize every property it can with empty values. For example, the following class structure will resolve as shown below?

```csharp
var customers = MagicFactory.For<Customer>()
    .WithBehavior(new FillWithEmptyValuesBehavior())
    .Many(2)
    .Build();
   
customers[0].Name.Should().Be("");
customers[0].Age.Should().Be(0);
customers[0].Address.ZipCode.Should().Be("");
customers[1].Name.Should().Be("");
customers[1].Age.Should().Be(0);
customers[1].Address.ZipCode.Should().Be("0");

public class Customer
{
    public string Name { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string ZipCode { get; set; }
}
```

### Global settings

You can set a default behavior for an entire project using `ForeverFactoryGlobalSettings`:

```csharp
// this needs to be executed before any tests
ForeverFactoryGlobalSettings
    .UseBehavior(new FillWithSequentialValuesBehavior(options =>
    {
        options.DateTimeOptions = new DateTimeSequenceOptions
        {
            DateTimeIncrements = DateTimeIncrements.Hours,
            StartDate = 2.September(2020)
        };
        options.FillNullables = false;
        options.Recursive = false;
    }));
    
var instances = MagicFactory.For<ClassA>().Many(2).Build().ToArray();

var secondInstance = instances[1];     
secondInstance.DateTimeProperty.Should().Be(2.September(2020).At(1.Hours()));
secondInstance.NullableDateTimeProperty.Should().BeNull("FillNullables option is set to false");
secondInstance.B.Should().BeNull("Recursive option is set to false");
```

You can always override the behavior for a specific scenario:

```csharp
 var instance = MagicFactory.For<ClassA>()
    .WithBehavior(new DoNotFillBehavior())
    .Build();
 
 instance.Name.Should().BeNull();
```

## How to contribute

You can help this project in many ways. Here are some ideas:

- Improving tests
- Reporting issues
- Making pull requests

### Running mutation tests

To assess the test suite quality, we use mutation testing with the tool Stryker.NET. After cloning this repository, you can install it by running the following command:  

```bash
dotnet tool restore
```

Then, you can navigate to the test project directory and then run stryker: 

```bash
cd ./tests/ForeverFactory.Tests
dotnet stryker
```

## Roadmap
 
- Elaborate examples combining ForeverFactory with **Faker**-_like_ libraries, like:
  - [bchavez/Bogus](https://github.com/bchavez/Bogus)
  - [jonwingfield/Faker.Net](https://github.com/jonwingfield/Faker.Net)
  - [mrstebo/FakerDotNet](https://github.com/mrstebo/FakerDotNet)
  - [Kuree/Faker.Net](https://github.com/Kuree/Faker.Net)
- Add support for void actions like `.Do(x => {})`
- Rewrite README; make it more exciting 
- **\[Breaking Change\]** Change Build() for returning an IList<T> instead of an IEnumerable<T>. This will avoid the inconvenience of having to cast to list or array in tests.
- Allow to configure custom builder per builder (single builder and many builder)
- Support custom constructor scoped by builder (for now, custom constructors are shared along the linked builders)
- "Smart" behavior, which identifies by convention which type of sequences and rules to apply to every property 
