using System;
using FluentAssertions;
using ForeverFactory.Generators.Transforms.Guards.Specifications;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Guards.Specifications
{
    public class ApplyTransformToLastInstancesSpecificationTests
    {
        [Theory]
        [InlineData(0, 2, 4, false)]
        [InlineData(1, 2, 4, false)]
        [InlineData(2, 2, 4, true)]
        [InlineData(3, 2, 4, true)]
        public void It_should_apply_only_to_the_last_n_instances(int currentIndex, int countToApply, int targetCount,
            bool expected)
        {
            var guardSpecification = new ApplyTransformToLastInstancesSpecification(countToApply, targetCount);

            var canApply = guardSpecification.CanApply(currentIndex);

            canApply.Should().Be(expected);
        }
        
        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void It_should_throw_if_count_to_apply_to_last_n_instances_is_greater_than_target_count(int countToApply, int targetCount)
        {
            Action invalidConfigurationBuild =
                () => new ApplyTransformToLastInstancesSpecification(countToApply, targetCount);

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Theory]
        [InlineData(-10)]
        [InlineData(-1)]
        public void It_should_throw_if_count_to_apply_to_last_n_instances_is_negative(int countToApply)
        {
            Action invalidConfigurationBuild =
                () => new ApplyTransformToLastInstancesSpecification(countToApply, targetCount: 2);

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations to negative counts");
        }
    }
}