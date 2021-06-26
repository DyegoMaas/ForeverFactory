using System;
using System.Linq;
using FluentAssertions;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Factories
{
    public class MultipleInstanceBuildingTests
    {
        [Fact]
        public void Should_throw_an_argument_exception_for_first_count_bigger_than_total_size()
        {
            Action invalidConfigurationBuild = () => new ProductFactory()
                .Many(10)
                .WithFirst(count: 11, x => x.Description = "xxx")
                .Build();

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
            
        [Fact]
        public void Should_throw_an_argument_exception_for_last_count_bigger_than_total_size()
        {
            Action invalidConfigurationBuild = () => new ProductFactory()
                .Many(10)
                .WithLast(count: 11, x => x.Description = "xxx")
                .Build();

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
            
        [Fact]
        public void WithFirst_applies_transformation_only_over_the_first_n_instances_produced()
        {
            var songs = new ProductFactory()
                .Many(10)
                .With(x => x.Description = "Dies Irae")
                .WithFirst(count: 2, x => x.Description = "Requiem II: Dies Irae")
                .Build()
                .ToList();
            
            var firstTwo = songs.Take(2);
            foreach (var song in firstTwo)
            {
                song.Description.Should().Be("Requiem II: Dies Irae");
            }
                
            var lastEight = songs.Skip(2);
            foreach (var song in lastEight)
            {
                song.Description.Should().Be("Dies Irae");
            }
        }
        
        [Fact]
        public void WithFirst_transforms_are_applied_in_order_overriding_previous_ones()
        {
            var products = new ProductFactory()
                .Many(10)
                .With(x => x.Description = "Default description")
                .WithFirst(5, x => x.Description = "Original first five")
                .WithFirst(2, x => x.Description = "New first two")
                .Build()
                .ToArray();

            var firstTwo = products.Take(2).Select(x => x.Description);
            firstTwo.Should().OnlyContain(x => x == "New first two");
            
            var nextThree = products.Skip(2).Take(3).Select(x => x.Description);
            nextThree.Should().OnlyContain(x => x == "Original first five");
            
            var lastFive = products.Skip(5).Select(x => x.Description);
            lastFive.Should().OnlyContain(x => x == "Default description");
        }
        
        [Fact]
        public void WithLast_transforms_are_applied_in_order_overriding_previous_ones()
        {
            var products = new ProductFactory()
                .Many(10)
                .With(x => x.Description = "Default description")
                .WithLast(5, x => x.Description = "Original last five")
                .WithLast(2, x => x.Description = "New last two")
                .Build()
                .ToArray();
            
            var firstFive = products.Take(5).Select(x => x.Description);
            firstFive.Should().OnlyContain(x => x == "Default description");

            var nextThree = products.Skip(5).Take(3).Select(x => x.Description);
            nextThree.Should().OnlyContain(x => x == "Original last five");
            
            var lastTwo = products.Skip(8).Select(x => x.Description);
            lastTwo.Should().OnlyContain(x => x == "New last two");
        }
            
        [Fact]
        public void WithLast_applies_transformation_only_over_the_last_x_instances_produced()
        {
            var songs = new ProductFactory()
                .Many(10)
                .With(x => x.Description = "Dies Irae")
                .WithLast(count: 2, x => x.Description = "Requiem II: Dies Irae")
                .Build()
                .ToList();
            
            songs.Should().HaveCount(10);
            
            var firstEight = songs.Take(8);
            foreach (var song in firstEight)
            {
                song.Description.Should().Be("Dies Irae");
            }
                
            var lastTwo = songs.Skip(8);
            foreach (var song in lastTwo)
            {
                song.Description.Should().Be("Requiem II: Dies Irae");
            }
        }
    }
}