using FluentAssertions;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Generators.Transforms
{
    public class ReflectedTransformTests
    {
        [Fact]
        public void It_should_apply_the_transform()
        {
            var funcTransform = new ReflectedFuncTransform<Person>((person, index) =>
            {
                person.FirstName = "Maria";
                person.LastName = "Valentina";
                person.Age = 10;
                return person;
            });
            var person = new Person();

            funcTransform.ApplyTo(person);

            person.FirstName.Should().Be("Maria");
            person.LastName.Should().Be("Valentina");
            person.Age.Should().Be(10);
        }
    }
}