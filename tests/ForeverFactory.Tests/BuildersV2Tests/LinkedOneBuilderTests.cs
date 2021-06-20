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
    public class LinkedBuilderTests
    {
        [Fact]
        public void It_should_apply_all_transforms_configured_through_With_method()
        {
            var linkedOneBuilder = GetBuilderFor<TravelLog>(defaultTransforms: null, customConstructor: null);

            var travelLog = linkedOneBuilder
                .With(x => x.Destination = "Maui")
                .With(x => x.PictureUrl = "https://somedomain/user/pictures/1234")
                .With(x => x.StartDate = 10.May(2020))
                .With(x => x.EndDate = 20.May(2020))
                .Build()
                .First();

            travelLog.Destination.Should().Be("Maui");
            travelLog.PictureUrl.Should().Be("https://somedomain/user/pictures/1234");
            travelLog.StartDate.Should().Be(10.May(2020));
            travelLog.EndDate.Should().Be(20.May(2020));
        }
        
        [Fact]
        public void It_should_apply_no_transforms_if_none_is_set()
        {
            var linkedOneBuilder = GetBuilderFor<TravelLog>(defaultTransforms: null, customConstructor: null);

            var travelLog = linkedOneBuilder.Build().First();

            travelLog.Destination.Should().BeNull();
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(default);
            travelLog.EndDate.Should().Be(default);
        }
        
        [Fact]
        public void It_should_apply_use_constructor_if_set()
        {
            var linkedOneBuilder = GetBuilderFor(
                customConstructor: () => new TravelLog { StartDate = DateTime.Today + 30.Days() }
            );
            
            var travelLog = linkedOneBuilder.Build().First();

            travelLog.Destination.Should().BeNull();
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(DateTime.Today + 30.Days());
            travelLog.EndDate.Should().Be(default);
        }
        
        [Fact]
        public void It_should_apply_default_transforms_if_set()
        {
            var linkedOneBuilder = GetBuilderFor(
                defaultTransforms: new Transform<TravelLog>[]
                {
                    FuncTransform<TravelLog, string>(x => x.Destination = "Hawaii"),
                    FuncTransform<TravelLog, DateTime>(x => x.StartDate = DateTime.Today + 30.Days()),
                }
            );

            var travelLog = linkedOneBuilder.Build().First();

            travelLog.Destination.Should().Be("Hawaii");
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(DateTime.Today + 30.Days());
            travelLog.EndDate.Should().Be(default);
        }

        [Fact]
        public void It_should_override_default_transforms_with_transforms_set_via_the_With_method()
        {
            var linkedOneBuilder = GetBuilderFor(
                defaultTransforms: new Transform<TravelLog>[]
                {
                    FuncTransform<TravelLog, string>(x => x.Destination = "Hawaii"),
                } 
            );

            var travelLog = linkedOneBuilder
                .With(x => x.Destination = "Las Vegas")
                .Build().First();

            travelLog.Destination.Should().Be("Las Vegas");
        }
        
        [Fact]
        public void It_should_create_only_one_instance_if_not_linked_with_more_builders()
        {
            var linkedOneBuilder = GetBuilderFor<TravelLog>(previous: null);

            var travelLogs = linkedOneBuilder.Build();

            travelLogs.Should().HaveCount(1, "it is not linked with others");
        }
        
        [Fact]
        public void It_should_create_only_three_instances_when_linked_to_two_more_builders()
        {
            var linkedOneBuilderA = GetBuilderFor<TravelLog>(previous: null);
            var linkedOneBuilderB = GetBuilderFor<TravelLog>(previous: linkedOneBuilderA);
            var linkedOneBuilderC = GetBuilderFor<TravelLog>(previous: linkedOneBuilderB);

            var travelLogs = linkedOneBuilderC.Build();

            travelLogs.Should().HaveCount(3, "there are three builders linked");
        }
        
        [Fact]
        public void All_linked_builders_should_share_the_same_building_context()
        {
            var builder = GetBuilderFor<TravelLog>(
                customConstructor: () => new TravelLog { StartDate = 20.November(2022)},
                defaultTransforms: new []
                {
                    FuncTransform<TravelLog, string>(x => x.Destination = "Blumenau")
                }
            );

            var travelLogs = builder
                .PlusOne()
                .PlusOne()
                .Build();

            travelLogs.Should().HaveCount(3, "there are three builders linked");
            foreach (var log in travelLogs)
            {
                log.StartDate.Should().Be(20.November(2022));
                log.Destination.Should().Be("Blumenau");
            }
        }
        
        [Fact]
        public void It_should_link_to_a_LinkedManyBuilder_with_method_Plus()
        {
            var builder = GetBuilderFor<TravelLog>();

            IManyBuilder<TravelLog> newBuilder = builder.Plus(5);

            newBuilder.Should().BeOfType<LinkedManyBuilder<TravelLog>>();
        }
        
        [Fact]
        public void It_should_link_to_a_LinkedOneBuilder_with_method_Plus()
        {
            var builder = GetBuilderFor<TravelLog>();

            ILinkedOneBuilder<TravelLog> newBuilder = builder.PlusOne();

            newBuilder.Should().BeOfType<LinkedOneBuilder<TravelLog>>();
        }

        private static LinkedOneBuilder<T> GetBuilderFor<T>(
            IEnumerable<Transform<T>> defaultTransforms = null, 
            Func<T> customConstructor = null,
            ILinkedBuilder<T> previous = null) 
            where T : class
        {
            var transformList = new TransformList<T>();
            transformList.AddRange(defaultTransforms ?? Enumerable.Empty<Transform<T>>());
            
            var sharedContext = new SharedContext<T>(transformList, customConstructor);
            return new LinkedOneBuilder<T>(sharedContext, previous);
        }

        private class TravelLog
        {
            public string Destination { get; set; }
            public string PictureUrl { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}