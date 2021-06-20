using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Builders;
using ForeverFactory.Builders.Common;
using ForeverFactory.Transforms;
using Xunit;
using static ForeverFactory.Tests.Helpers.TestHelpers;

namespace ForeverFactory.Tests.BuildersV2Tests
{
    public class OneBuilderTests
    {
        [Fact]
        public void It_should_apply_all_transforms_configured_through_With_method()
        {
            var oneBuilder = GetBuilderFor<TravelLog>(defaultTransforms: null, customConstructor: null);

            var travelLog = oneBuilder
                .With(x => x.Destination = "Maui")
                .With(x => x.PictureUrl = "https://somedomain/user/pictures/1234")
                .With(x => x.StartDate = 10.May(2020))
                .With(x => x.EndDate = 20.May(2020))
                .Build();

            travelLog.Destination.Should().Be("Maui");
            travelLog.PictureUrl.Should().Be("https://somedomain/user/pictures/1234");
            travelLog.StartDate.Should().Be(10.May(2020));
            travelLog.EndDate.Should().Be(20.May(2020));
        }
        
        [Fact]
        public void It_should_apply_all_transforms_configured_through_With_method_using_a_transform()
        {
            var oneBuilder = GetBuilderFor<TravelLog>(defaultTransforms: null, customConstructor: null);

            var travelLog = oneBuilder
                .With(FuncTransform<TravelLog, string>(x => x.Destination = "Texas"))
                .Build();

            travelLog.Destination.Should().Be("Texas");
        }
        
        [Fact]
        public void It_should_apply_no_transforms_if_none_is_set()
        {
            var oneBuilder = GetBuilderFor<TravelLog>(defaultTransforms: null, customConstructor: null);

            var travelLog = oneBuilder.Build();

            travelLog.Destination.Should().BeNull();
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(default);
            travelLog.EndDate.Should().Be(default);
        }
        
        [Fact]
        public void It_should_apply_use_constructor_if_set()
        {
            var oneBuilder = GetBuilderFor(
                customConstructor: () => new TravelLog { StartDate = DateTime.Today + 30.Days() }
            );
            
            var travelLog = oneBuilder.Build();

            travelLog.Destination.Should().BeNull();
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(DateTime.Today + 30.Days());
            travelLog.EndDate.Should().Be(default);
        }
        
        [Fact]
        public void It_should_apply_default_transforms_if_set()
        {
            var oneBuilder = GetBuilderFor(
                defaultTransforms: new Transform<TravelLog>[]
                {
                    FuncTransform<TravelLog, string>(x => x.Destination = "Hawaii"),
                    FuncTransform<TravelLog, DateTime>(x => x.StartDate = DateTime.Today + 30.Days()),
                }
            );

            var travelLog = oneBuilder.Build();

            travelLog.Destination.Should().Be("Hawaii");
            travelLog.PictureUrl.Should().BeNull();
            travelLog.StartDate.Should().Be(DateTime.Today + 30.Days());
            travelLog.EndDate.Should().Be(default);
        }

        [Fact]
        public void It_should_override_default_transforms_with_transforms_set_via_the_With_method()
        {
            var oneBuilder = GetBuilderFor(
                defaultTransforms: new Transform<TravelLog>[]
                {
                    FuncTransform<TravelLog, string>(x => x.Destination = "Hawaii"),
                } 
            );

            var travelLog = oneBuilder
                .With(x => x.Destination = "Las Vegas")
                .Build();

            travelLog.Destination.Should().Be("Las Vegas");
        }

        private static OneBuilder<T> GetBuilderFor<T>(IEnumerable<Transform<T>> defaultTransforms = null, Func<T> customConstructor = null) 
            where T : class
        {
            var transformList = new TransformList<T>();
            transformList.AddRange(defaultTransforms ?? Enumerable.Empty<Transform<T>>());
            
            var sharedContext = new SharedContext<T>(transformList, customConstructor);
            return new OneBuilder<T>(sharedContext);
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