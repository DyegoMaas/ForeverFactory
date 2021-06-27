using FluentAssertions;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms
{
    public class FuncTransformTests
    {
        [Fact]
        public void It_should_apply_the_transform()
        {
            var funcTransform = new FuncTransform<Person, string>(x => x.FirstName = "Maria");
            var person = new Person();

            funcTransform.ApplyTo(person);

            person.FirstName.Should().Be("Maria");
        }
    }
}