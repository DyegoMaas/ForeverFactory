# Forever Factory

![License](https://img.shields.io/github/license/DyegoMaas/ForeverFactory.svg)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/DyegoMaas/ForeverFactory/build-and-test?label=tests)
![Nuget](https://img.shields.io/nuget/v/ForeverFactory)
![Nuget](https://img.shields.io/nuget/dt/ForeverFactory)

**Forever Factory** is a .NET 5 library that helps you build custom objects. By merging design patterns like Factory and Builder, it makes it super easy to create hundreds of customized objects.

With Forever Factory, building a new object can be as simple as `MagicFactory.For<Person>().Build()`.

## Building a single object

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

## Building a multiple objects

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
    .Build(); // creates 10 persons with age 5, and 20 with age 60
```

## Custom Factories

You create your own factory, with predefined rules, like this:

```csharp
public class PersonFactory : MagicFactory<Person>
{
    public PersonFactory()
    {
        Set(x => x.FirstName = "Albert");
        Set(x => x.LastName = "Einstein");
        Set(x => x.Age = 56);
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

## Custom constructors

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
    public ProductFactory()
    {
        UseConstructor(() => new Product("Nimbus 2000", "Brooms"));
        Set(x => x.Description = "Top of the line flying broom");
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

## Roadmap

- Support multi-level object creation
- Add automatic SemVer bumping
- The default factory could have an option to fill all properties with valid values
- Allow configuration by rules
- Allow sequences in numbers properties and things like email, etc
- Support group configurations (WithFirst and WithLast) in the constructor of a custom factory
