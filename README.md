# Forever Factory - Build test objects quickly

![License](https://img.shields.io/github/license/DyegoMaas/ForeverFactory.svg)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/DyegoMaas/ForeverFactory/build-and-test?label=tests)
[![codecov](https://codecov.io/gh/DyegoMaas/ForeverFactory/branch/main/graph/badge.svg?token=IPBG3BP2D8)](https://codecov.io/gh/DyegoMaas/ForeverFactory)
![Nuget](https://img.shields.io/nuget/v/ForeverFactory)
![Nuget](https://img.shields.io/nuget/dt/ForeverFactory)

**Forever Factory** helps you build custom objects. By smartly merging design patterns like Factory and Builder, it makes it super easy to create hundreds of customized objects.

With Forever Factory, building a new object can be as simple as `MagicFactory.For<Person>().Build()`.

## How to use?

### Installing ForeverFactory

You may install ForeverFactory with NuGet:

`Install-Package ForeverFactory` Teste

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
    public int Age { get; set; }
}
```

We can build a new instance of this class as follows:

```csharp
var person = MagicFactory.For<Person>().Build();
```

And we can customize each field:

```csharp
var vitorHugo = MagicFactory.For<Person>()
    .With(x => x.FirstName = "Vitor")
    .With(x => x.LastName = "Hugo")
    .Build();
```

### Building a multiple objects

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
var hundredPeople = MagicFactory.For<Person>()
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

By default, it will not fill any properties, and it is up to you to fill any properties.

#### FillWithEmptyValuesBehavior

With this behavior, ForeverFactory will recursively initialize every property it can with empty values. For example, the following class structure will resolve as shown below?

```csharp
public class Customer
{
    public string Name { get; set; } // will be set to ""
    public Address Address { get; set; } // will be set to 'new Address()' 
}

public class Address
{
    public string ZipCode { get; set; } // will be set to ""
}
```

#### FillWithSequentialValuesBehavior

With this behavior, ForeverFactory will recursively initialize every property it can with sequential values. This is similar to the default behavior of NBuilder:

```csharp
var people = MagicFactory.For<ClassWithInteger>().Many(100).Build();
people[0].Name.Should().Be("Name1");
people[0].Age.Should().Be(1);
people[0].Address.ZipCode.Should().Be("ZipCode1");
people[1].Name.Should().Be("Name2");
people[1].Age.Should().Be(2);
people[1].Address.ZipCode.Should().Be("ZipCode2");

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


## How fast is it?

The table below shows test done with Benchmark Dotnet tool comparing equivalent scenarios in both `Forever Factory` and `NBuilder`:

```csv
|                                                    Method |           Mean |       Error |      StdDev |   Gen 0 |   Gen 1 | Allocated |
|---------------------------------------------------------- |---------------:|------------:|------------:|--------:|--------:|----------:|
|                           BuildSingleObjectForeverFactory |       683.7 ns |     7.53 ns |     7.04 ns |  0.1373 |       - |   1,152 B |
|                                 BuildSingleObjectNBuilder |     1,862.1 ns |    13.56 ns |    12.68 ns |  0.0935 |       - |     784 B |
|                        BuildThousandObjectsForeverFactory |   248,116.4 ns | 2,383.28 ns | 2,112.72 ns | 53.7109 |  5.8594 | 449,403 B |
|                              BuildThousandObjectsNBuilder | 1,539,214.8 ns | 8,576.28 ns | 7,602.65 ns | 78.1250 | 15.6250 | 653,403 B |
| BuildThousandObjectsFillingSequentialValuesForeverFactory |   575,005.3 ns | 4,403.68 ns | 4,119.20 ns | 80.0781 | 15.6250 | 673,114 B |
|       BuildThousandObjectsFillingSequentialValuesNBuilder | 1,017,233.2 ns | 8,782.64 ns | 7,785.58 ns | 70.3125 | 13.6719 | 589,285 B |
```

The code to the tests is found [here](tests/Benchmarks/Program.cs).

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

- Allow to configure custom builder per builder (single builder and many builder)
- Support custom constructor scoped by builder (for now, custom constructors are shared along the linked builders)
- Allow configuration by rules
- Allow sequences in numbers properties and things like email, etc
- Support "smart" behavior, which identifies by convention which type of sequences and rules to apply to every property 
- Add the concept of "Localization Extensions", which could contain localized versions of the fluent API, translated for other languages, like, portuguese, spanish, etc