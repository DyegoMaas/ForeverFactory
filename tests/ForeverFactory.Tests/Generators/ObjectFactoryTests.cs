using FluentAssertions;
using ForeverFactory.Customizations;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Generators
{
    public class ObjectFactoryTests
    {
        private readonly ObjectFactory<Person> _factory;

        public ObjectFactoryTests()
        {
            var customizeFactoryOptions = new OptionsCollector<Person>(customization => {});
            _factory = new ObjectFactory<Person>(customizeFactoryOptions);
        }

        [Fact]
        public void It_should_build_an_enumerable_if_no_nodes_are_added()
        {
            var persons = _factory.Build();

            persons.Should().NotBeNull();
        }

        [Fact]
        public void It_should_build_upon_the_added_generator_nodes()
        {
            _factory.AddNode(new GeneratorNode<Person>(1));
            _factory.AddNode(new GeneratorNode<Person>(2));
            var persons = _factory.Build();

            persons.Should().HaveCount(3);
        }

        [Fact]
        public void It_should_clear_nodes_when_adding_root_node()
        {
            _factory.AddNode(new GeneratorNode<Person>(1));
            _factory.AddRootNode(new GeneratorNode<Person>(2));

            var persons = _factory.Build();

            persons.Should().HaveCount(2, "the other node was deleted when the new root was added");
        }

        [Fact]
        public void It_should_apply_default_transforms_to_all_generator_nodes()
        {
            var customizeFactoryOptions = new OptionsCollector<Person>(customization =>
            {
                customization.Set(x => x.FirstName = "Clark");
            });

            var factory = new ObjectFactory<Person>(customizeFactoryOptions);
            factory.AddNode(new GeneratorNode<Person>(1));
            factory.AddNode(new GeneratorNode<Person>(2));

            var persons = factory.Build();

            foreach (var person in persons) 
                person.FirstName.Should().Be("Clark");
        }

        // TODO refactor
        [Fact]
        public void It_should_apply_default_transforms_to_all_generator_nodes2()
        {
            var customizeFactoryOptions = new OptionsCollector<Person>(initialization =>
            {
                initialization.Set(x => x.FirstName = "Clark");
            });

            var factory = new ObjectFactory<Person>(customizeFactoryOptions);
            factory.AddNode(new GeneratorNode<Person>(1));
            factory.AddNode(new GeneratorNode<Person>(2));

            var persons = factory.Build();

            foreach (var person in persons) 
                person.FirstName.Should().Be("Clark");
        }
    }
}