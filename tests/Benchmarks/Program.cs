// See https://aka.ms/new-console-template for more information

using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmarks;
using FizzWare.NBuilder;
using ForeverFactory;
using ForeverFactory.Behaviors;

var summary = BenchmarkRunner.Run(typeof(BuildersBenchmark));

namespace Benchmarks
{
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
        public void BuildThousandObjectsFillingSequentialValuesForeverFactory()
        {
            MagicFactory.For<Person>()
                .WithBehavior(new FillWithSequentialValuesBehavior(options => options.Recursive = false))
                .Many(1000).Build().ToList();
        }
    
        [Benchmark]
        public void BuildThousandObjectsFillingSequentialValuesNBuilder()
        {
            Builder<Person>.CreateListOfSize(1000).All().Build();
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }
}