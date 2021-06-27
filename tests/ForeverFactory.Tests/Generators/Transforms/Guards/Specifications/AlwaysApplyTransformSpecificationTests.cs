using FluentAssertions;
using ForeverFactory.Generators.Transforms.Guards.Specifications;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms.Guards.Specifications
{
    public class AlwaysApplyTransformSpecificationTests
    {
        [Fact]
        public void It_should_apply_if_no_conditions_were_set()
        {
            var guardSpecification = new AlwaysApplyTransformSpecification();

            var canApply = guardSpecification.CanApply(0);

            canApply.Should().BeTrue();
        }

 

       
    }
}