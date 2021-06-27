﻿using FluentAssertions;
using ForeverFactory.Generators;
using ForeverFactory.Generators.Transforms;
using ForeverFactory.Tests.Factories.CustomizedFactories.ExampleFactories;
using Xunit;

namespace ForeverFactory.Tests.Generators
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

            factory.AddNode(new GeneratorNode<Person>(1));
            factory.AddNode(new GeneratorNode<Person>(2));
            var persons = factory.Build();

            persons.Should().HaveCount(3);
        }

        [Fact]
        public void It_should_apply_default_transforms_to_all_generator_nodes()
        {
            var factory = new ObjectFactory<Person>();
            factory.AddNode(new GeneratorNode<Person>(1));
            factory.AddNode(new GeneratorNode<Person>(2));
            factory.AddDefaultTransform(new FuncTransform<Person, string>(x => x.FirstName = "Clark"));

            var persons = factory.Build();

            foreach (var person in persons) person.FirstName.Should().Be("Clark");
        }

        [Fact]
        public void It_should_clear_nodes_when_adding_root_node()
        {
            var factory = new ObjectFactory<Person>();
            factory.AddNode(new GeneratorNode<Person>(1));
            factory.AddRootNode(new GeneratorNode<Person>(2));

            var persons = factory.Build();

            persons.Should().HaveCount(2, "the other node was deleted when the new root was added");
        }
    }
}