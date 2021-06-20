using FluentAssertions;
using ForeverFactory.Core;
using ForeverFactory.Core.Transforms;
using ForeverFactory.Tests.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Core
{
    public class ObjectFactoryTests
    {
        [Fact]
        public void It_should_build_an_enumerable_if_no_nodes_are_added()
        {
            var factory = new ObjectFactory<Person>();

            var persons = factory.Build();

            persons.Should().NotBeNull();
        }

        [Fact]
        public void It_should_build_upon_the_added_generator_nodes()
        {
            var factory = new ObjectFactory<Person>();

            factory.AddNode(new GeneratorNode<Person>(targetCount: 1));
            factory.AddNode(new GeneratorNode<Person>(targetCount: 2));
            var persons = factory.Build();

            persons.Should().HaveCount(3);
        }

        [Fact]
        public void It_should_apply_default_transforms_to_all_generator_nodes()
        {
            var factory = new ObjectFactory<Person>();
            factory.AddNode(new GeneratorNode<Person>(targetCount: 1));
            factory.AddNode(new GeneratorNode<Person>(targetCount: 2));
            factory.AddDefaultTransform(new FuncTransform<Person, string>(x => x.FirstName = "Clark"));

            var persons = factory.Build();

            foreach (var person in persons)
            {
                person.FirstName.Should().Be("Clark");
            }
        }

        [Fact]
        public void It_should_return_the_current_generator_node()
        {
            var factory = new ObjectFactory<Person>();
            var generatorNode1 = new GeneratorNode<Person>(targetCount: 1);
            var generatorNode2 = new GeneratorNode<Person>(targetCount: 2);
            
            factory.AddNode(generatorNode1);
            factory.GetCurrentGeneratorNode().Should().Be(generatorNode1);
            
            factory.AddNode(generatorNode2);
            factory.GetCurrentGeneratorNode().Should().Be(generatorNode2);
        }
        
        [Fact]
        public void It_should_clear_nodes_when_adding_root_node()
        {
            var factory = new ObjectFactory<Person>();
            factory.AddNode(new GeneratorNode<Person>(targetCount: 1));
            factory.AddRootNode(new GeneratorNode<Person>(targetCount: 2));

            var persons = factory.Build();

            persons.Should().HaveCount(2, "the other node was deleted when the new root was added");
        }
    }
}