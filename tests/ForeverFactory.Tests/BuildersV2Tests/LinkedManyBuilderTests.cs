using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Builders;
using ForeverFactory.Builders.Common;
using ForeverFactory.Transforms;
using static ForeverFactory.Tests.Helpers.TestHelpers;
using Xunit;

namespace ForeverFactory.Tests.BuildersV2Tests
{
    public class LinkedManyBuilderTests
    {
        [Fact]
        public void It_should_produce_the_correct_number_of_instances()
        {
            var builder = GetBuilderFor<Song>(count: 10, defaultTransforms: null, customConstructor: null);
            
            var persons = builder.Build()
                .ToList();

            persons.Should().HaveCount(10);
        }
        
            
        [Fact]
        public void It_produces_many_instances_with_overwritten_properties()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            var songs = builder
                .With(x => x.Name = "Requiem II: Dies Irae")
                .With(x => x.Artist = "Giuseppe Verdi")
                .Build()
                .ToList();
        
            songs.Should().HaveCount(10);
            foreach (var song in songs)
            {
                song.Name.Should().Be("Requiem II: Dies Irae");
                song.Artist.Should().Be("Giuseppe Verdi");
                song.PublishDate.Should().Be(default);
            }
        }
        
        [Fact]
        public void Should_throw_an_argument_exception_for_first_count_bigger_than_total_size()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            Action invalidConfigurationBuild = () => builder
                .WithFirst(count: 11, x => x.PublishDate = 1.January(1500))
                .Build();
        
            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void Should_not_throw_an_argument_exception_for_first_if_count_is_same_or_less_than_total_size()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            Action invalidConfigurationBuild = () => builder
                .WithFirst(count: 10, x => x.PublishDate = 1.January(1500))
                .Build();
        
            invalidConfigurationBuild.Should()
                .NotThrow<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void Should_throw_an_argument_exception_for_last_count_bigger_than_total_size()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            Action invalidConfigurationBuild = () => builder
                .WithLast(count: 11, x => x.PublishDate = 1.January(1500))
                .Build();
        
            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void Should_not_throw_an_argument_exception_for_last_if_count_is_same_or_less_than_total_size()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            Action invalidConfigurationBuild = () => builder
                .WithLast(count: 10, x => x.PublishDate = 1.January(1500))
                .Build();
        
            invalidConfigurationBuild.Should()
                .NotThrow<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Fact]
        public void WithFirst_applies_transformation_only_over_the_first_n_instances_produced()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            var songs = builder
                .With(x => x.Name = "Dies Irae")
                .With(x => x.Artist = "Giuseppe Verdi")
                .WithFirst(count: 2, x => x.Name = "Requiem II: Dies Irae")
                .Build()
                .ToList();
        
            var firstTwo = songs.Take(2);
            foreach (var song in firstTwo)
            {
                song.Name.Should().Be("Requiem II: Dies Irae");
                song.Artist.Should().Be("Giuseppe Verdi");
            }
            
            var lastEight = songs.Skip(2);
            foreach (var song in lastEight)
            {
                song.Name.Should().Be("Dies Irae");
                song.Artist.Should().Be("Giuseppe Verdi");
            }
        }
        
        [Fact]
        public void WithLast_applies_transformation_only_over_the_last_x_instances_produced()
        {
            var builder = GetBuilderFor<Song>(count: 10);
            
            var songs = builder
                .With(x => x.Name = "Dies Irae")
                .With(x => x.Artist = "Giuseppe Verdi")
                .With(x => x.PublishDate = 1.April(1500))
                .WithLast(count: 2, x => x.Name = "Requiem II: Dies Irae")
                .WithLast(count: 3, x => x.PublishDate = 1.April(1505))
                .Build()
                .ToList();
        
            songs.Should().HaveCount(10);
        
            var firstSeven = songs.Take(7);
            foreach (var song in firstSeven)
            {
                song.Name.Should().Be("Dies Irae");
                song.Artist.Should().Be("Giuseppe Verdi");
                song.PublishDate.Should().Be(1.April(1500));
            }
            
            var seventhSong = songs.Skip(7).First();
            seventhSong.Name.Should().Be("Dies Irae");
            seventhSong.Artist.Should().Be("Giuseppe Verdi");
            seventhSong.PublishDate.Should().Be(1.April(1505));
            
            var lastTwo = songs.Skip(8);
            foreach (var song in lastTwo)
            {
                song.Name.Should().Be("Requiem II: Dies Irae");
                song.Artist.Should().Be("Giuseppe Verdi");
                song.PublishDate.Should().Be(1.April(1505));
            }
        }
        
        [Fact]
        public void It_should_apply_use_constructor_if_set()
        {
            var builder = GetBuilderFor<Song>(count: 5,
                customConstructor: () => new Song { PublishDate = 2.November(2000) }
            );
            
            var songs = builder.Build();
        
            songs.Should().HaveCount(5);
            foreach (var song in songs)
            {
                song.PublishDate.Should().Be(2.November(2000));
            }
        }
        
        [Fact]
        public void It_should_apply_default_transforms_if_set()
        {
            var builder = GetBuilderFor(count: 5,
                defaultTransforms: new Transform<Song>[]
                {
                    FuncTransform<Song, string>(x => x.Name = "Hallelujah"),
                    FuncTransform<Song, DateTime>(x => x.PublishDate = 2.April(1984)),
                }
            );
        
            var songs = builder.Build();

            foreach (var song in songs)
            {
                song.Name.Should().Be("Hallelujah");
                song.PublishDate.Should().Be(2.April(1984));
            }
        }
        
        [Fact]
        public void It_should_override_default_transforms_with_transforms_set_via_the_With_method()
        {
            var builder = GetBuilderFor(count: 5,
                defaultTransforms: new Transform<Song>[]
                {
                    FuncTransform<Song, string>(x => x.Name = "Hallelujah"),
                }
            );
        
            var song = builder
                .With(x => x.Name = "Dies Irae")
                .Build().First();
        
            song.Name.Should().Be("Dies Irae");
        }
        
        [Fact]
        public void It_should_create_only_the_instances_of_this_builder_if_not_linked_with_more_builders()
        {
            var builder = GetBuilderFor<Song>(count: 5, previous: null);
        
            var songs = builder.Build();
        
            songs.Should().HaveCount(5, "it is not linked with others");
        }
        
        [Fact]
        public void It_should_create_only_three_instances_when_linked_to_two_more_builders()
        {
            var linkedManyBuilderA = GetBuilderFor<Song>(count: 2, previous: null);
            var linkedManyBuilderB = GetBuilderFor<Song>(count: 2, previous: linkedManyBuilderA);
            var linkedManyBuilderC = GetBuilderFor<Song>(count: 2, previous: linkedManyBuilderB);
        
            var songs = linkedManyBuilderC.Build();
        
            songs.Should().HaveCount(6, "there are three linked builders that produce two instances each");
        }

        [Fact]
        public void All_linked_builders_should_share_the_same_building_context()
        {
            var builder = GetBuilderFor<Song>(count: 10,
                customConstructor: () => new Song {PublishDate = 20.November(2020)},
                defaultTransforms: new[]
                {
                    FuncTransform<Song, string>(x => x.Name = "Dies Irae")
                }
            );

            var songs = builder
                .Plus(10)
                .Plus(10)
                .Build();

            songs.Should().HaveCount(30, "there are three linked builders that produce 10 instances each");
            foreach (var song in songs)
            {
                song.PublishDate.Should().Be(20.November(2020));
                song.Name.Should().Be("Dies Irae");
            }
        }

        [Fact]
        public void It_should_generate_multiple_sets_of_configurable_instances2()
        {
            var builder = GetBuilderFor<Song>(count: 10,
                customConstructor: () => new Song
                {
                    Artist = "Daft Punk", 
                    Name = "Da Funk", 
                    PublishDate = 1.January(1995)
                }
            );
            
            var songs = builder
                .WithLast(count: 2, x => x.PublishDate = 1.January(1999))
                .WithLast(count: 2, x => x.Name = "Around the World")
                .Plus(count: 5)
                .WithFirst(count: 3, x => x.PublishDate = 1.January(2000))
                .With(x => x.Name = "Get Lucky")
                .Build()
                .ToList();

            songs.Should().HaveCount(15);

            var firstEight = songs.Take(8);
            foreach (var song in firstEight)
            {
                song.Artist.Should().Be("Daft Punk");
                song.Name.Should().Be("Da Funk");
                song.PublishDate.Should().Be(1.January(1995));
            }
        
            var nextTwo = songs.Skip(8).Take(2);
            foreach (var song in nextTwo)
            {
                song.Artist.Should().Be("Daft Punk");
                song.Name.Should().Be("Around the World");
                song.PublishDate.Should().Be(1.January(1999));
            }
        
            var nextThree = songs.Skip(10).Take(3);
            foreach (var song in nextThree)
            {
                song.Artist.Should().Be("Daft Punk");
                song.Name.Should().Be("Get Lucky");
                song.PublishDate.Should().Be(1.January(2000));
            }
        
            nextTwo = songs.TakeLast(2);
            foreach (var song in nextTwo)
            {
                song.Artist.Should().Be("Daft Punk");
                song.Name.Should().Be("Get Lucky");
                song.PublishDate.Should().Be(1.January(1995));
            }
        }
        
        [Fact]
        public void It_should_link_to_a_LinkedManyBuilder_with_method_Plus()
        {
            var builder = GetBuilderFor<Song>(count: 2);

            IManyBuilder<Song> newBuilder = builder.Plus(5);

            newBuilder.Should().BeOfType<LinkedManyBuilder<Song>>();
        }
        
        [Fact]
        public void It_should_link_to_a_LinkedOneBuilder_with_method_Plus()
        {
            var builder = GetBuilderFor<Song>(count: 2);

            ILinkedOneBuilder<Song> newBuilder = builder.PlusOne();

            newBuilder.Should().BeOfType<LinkedOneBuilder<Song>>();
        }
        
        private static LinkedManyBuilder<T> GetBuilderFor<T>(
            int count,
            IEnumerable<Transform<T>> defaultTransforms = null, 
            Func<T> customConstructor = null,
            ILinkedBuilder<T> previous = null) 
            where T : class
        {
            var transformList = new TransformList<T>();
            transformList.AddRange(defaultTransforms ?? Enumerable.Empty<Transform<T>>());
            
            var sharedContext = new SharedContext<T>(transformList, customConstructor);
            return new LinkedManyBuilder<T>(count, sharedContext, previous);
        }

        private class Song
        {
            public string Name { get; set; }
            public string Artist { get; set; }
            public DateTime PublishDate { get; set; }
        }
    }
}