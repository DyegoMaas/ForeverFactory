// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FizzWare.NBuilder;
using ForeverFactory;

var summary = BenchmarkRunner.Run(typeof(BuildersBenchmark));

[MemoryDiagnoser]
public class BuildersBenchmark
{
    private const string PersonName = "Arnold";

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
}

public class Person
{
    public string Name { get; set; }
}