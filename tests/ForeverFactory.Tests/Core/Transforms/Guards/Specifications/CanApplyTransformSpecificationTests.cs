using FluentAssertions;
using ForeverFactory.Core.Transforms.Guards.Specifications;
using Xunit;

namespace ForeverFactory.Tests.Core.Transforms.Guards.Specifications
{
    public class GuardedTransformsTests
    {
        [Fact]
        public void It_should_apply_if_no_conditions_were_set()
        {
            var guardSpecification = new AlwaysApplyTransformSpecification();
                
            var canApply = guardSpecification.CanApply(currentIndex: 0);
                
            canApply.Should().BeTrue();
        }
            
        [Theory]
        [InlineData(0, 2, true)]
        [InlineData(1, 2, true)]
        [InlineData(2, 2, false)]
        public void It_should_apply_only_to_the_first_n_instances(int currentIndex, int countToApply, bool expected)
        {
            var guardSpecification = new ApplyTransformToFirstInstancesSpecification(countToApply);
                
            var canApply = guardSpecification.CanApply(currentIndex);
                
            canApply.Should().Be(expected);
        }
            
        [Theory]
        [InlineData(0, 2, 4, false)]
        [InlineData(1, 2, 4, false)]
        [InlineData(2, 2, 4, true)]
        [InlineData(3, 2, 4, true)]
        public void It_should_apply_only_to_the_last_n_instances(int currentIndex, int countToApply, int targetCount, bool expected)
        {
            var guardSpecification = new ApplyTransformToLastInstancesSpecification(countToApply, targetCount);
                
            var canApply = guardSpecification.CanApply(currentIndex);
                
            canApply.Should().Be(expected);
        }
    }
}