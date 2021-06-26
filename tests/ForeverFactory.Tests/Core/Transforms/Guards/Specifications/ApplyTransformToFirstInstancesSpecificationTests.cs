using System;
using FluentAssertions;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using Xunit;

namespace ForeverFactory.Tests.Core.Transforms.Guards.Specifications
{
    public class ApplyTransformToFirstInstancesSpecificationTests
    {
        [Theory]
        [InlineData(0, 2, true)]
        [InlineData(1, 2, true)]
        [InlineData(2, 2, false)]
        public void It_should_apply_only_to_the_first_n_instances(int currentIndex, int countToApply, bool expected)
        {
            var guardSpecification = new ApplyTransformToFirstInstancesSpecification(countToApply, targetCount: 2);

            var canApply = guardSpecification.CanApply(currentIndex);

            canApply.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void It_should_throw_if_count_to_apply_to_first_n_instances_is_greater_than_target_count(int countToApply, int targetCount)
        {
            Action invalidConfigurationBuild =
                () => new ApplyTransformToFirstInstancesSpecification(countToApply, targetCount);

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations beyond set size");
        }
        
        [Theory]
        [InlineData(-10)]
        [InlineData(-1)]
        public void It_should_throw_if_count_to_apply_to_first_n_instances_is_negative(int countToApply)
        {
            Action invalidConfigurationBuild =
                () => new ApplyTransformToFirstInstancesSpecification(countToApply, targetCount: 2);

            invalidConfigurationBuild.Should()
                .Throw<ArgumentException>("it is not possible to apply transformations to negative counts");
        }
    }
}