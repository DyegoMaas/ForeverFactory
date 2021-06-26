using FluentAssertions;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Core.Transforms
{
    public class ReflectedTransformTests
    {
        [Fact]
        public void It_should_apply_the_transform()
        {
            var funcTransform = new ReflectedFuncTransform<Person>(x =>
            {
                x.FirstName = "Maria";
                x.LastName = "Valentina";
                x.Age = 10;
                return x;
            });
            var person = new Person();

            funcTransform.ApplyTo(person);

            person.FirstName.Should().Be("Maria");
            person.LastName.Should().Be("Valentina");
            person.Age.Should().Be(10);
        }
    }
}