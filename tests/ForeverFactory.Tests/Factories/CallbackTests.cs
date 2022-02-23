using FluentAssertions;
using Xunit;

namespace ForeverFactory.Tests.Factories;

public class CallbackTests
{
    [Fact]
    public void Should_expectation()
    {
        var person = MagicFactory.For<Person>()
            .Do(x => x.Email = $"{x.FirstName}.{x.LastName}@gmail.com".ToLower())
            
            
            .With(x => x.FirstName = "Albert")
            .With(x => x.LastName = "Einstein")
            .Build();

        person.Email.Should().Be("albert.einstein@gmail.com");
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}