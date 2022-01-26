// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FizzWare.NBuilder;
using ForeverFactory;
using ForeverFactory.Behaviors;

var classAs = Builder<ClassA>.CreateListOfSize(10).Build();

var summary = BenchmarkRunner.Run(typeof(BuildersBenchmark));

public class ClassA
{
    public string PropertyX { get; set; }
    public int intValue { get; set; }
    public float FloatValue { get; set; }
}

public class ClassB
{
    public string PropertyY { get; set; }
    public ClassC C { get; set; }
}
        
public class ClassC
{
    public string PropertyZ { get; set; }
}

[MemoryDiagnoser]
public class BuildersBenchmark
{
    public const string PersonName = "Arnold";

    [Benchmark]
    public void BuildSingleObjectForeverFactory()
    {
        MagicFactory.For<Person>().With(x => x.Name = PersonName).Build();
    }
    
    [Benchmark]
    public void BuildSingleObjectNBuilder()
    {
        Builder<Person>.CreateNew().With(x => x.Name = PersonName).Build();
    }
    
    [Benchmark]
    public void BuildThousandObjectsForeverFactory()
    {
        MagicFactory.For<Person>().Many(1000).With(x => x.Name = PersonName).Build().ToList();
    }
    
    [Benchmark]
    public void BuildThousandObjectsNBuilder()
    {
        Builder<Person>.CreateListOfSize(1000).All().With(x => x.Name = PersonName).Build();
    }
    
    [Benchmark]
    public void BuildThousandObjectsForeverFactoryFillingSequentialValues()
    {
        MagicFactory.For<Person>()
            .WithBehavior(new FillWithSequentialValuesBehavior())
            .Many(1000).Build().ToList();
    }
    
    [Benchmark]
    public void BuildThousandObjectsNBuilderFillingSequentialValues()
    {
        Builder<Person>.CreateListOfSize(1000).All().Build();
    }
}

public class Person
{
    public string Name { get; set; }
}