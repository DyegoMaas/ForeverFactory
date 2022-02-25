using System;
using System.Linq;
using FluentAssertions;
using ForeverFactory.FluentInterfaces;
using Xunit;

namespace ForeverFactory.Tests.Factories;

public class CallbackTests
{
    [Fact]
    public void should_execute_callbacks_in_order()
    {
        string shipNameBefore = null;
        string shipNameAfter = null;
        
        MagicFactory.For<Ship>()
            .Do(x => shipNameBefore = x.Name)
            .With(x => x.Name = "Mary")
            .Do(x => shipNameAfter = x.Name)
            .Build();

        shipNameBefore.Should().BeNull();
        shipNameAfter.Should().Be("Mary");
    }
    
    [Fact]
    public void should_execute_callback_through_ICustomizeOneBuildOne()
    {
        string shipName = null;
        
        Action<Ship> saveShipName = ship => shipName = ship.Name;
        ICustomizeOneBuildOne<Ship> factory = MagicFactory.For<Ship>()
            .With(x => x.Name = "Mary");
        factory
            .Do(saveShipName)
            .Build();

        shipName.Should().Be("Mary");
    }
    
    [Fact]
    public void should_execute_callback_through_ICustomizeManyBuildMany()
    {
        string shipName = null;
        var counter = 0;
        
        Action<Ship> saveShipName = ship =>
        {
            shipName = ship.Name;
            counter++;
        };
        ICustomizeManyBuildMany<Ship> factory = MagicFactory.For<Ship>()
            .Many(2)
            .With(x => x.Name = "Mary");
        factory
            .Do(saveShipName)
            .Build()
            .ToArray();

        shipName.Should().Be("Mary");
        counter.Should().Be(2);
    }
    
    [Fact]
    public void should_execute_callback_through_ICustomizeOneBuildOneWithNavigation()
    {
        string shipName = null;
        
        Action<Ship> saveShipName = ship => shipName = ship.Name;
        ICustomizeOneBuildOneWithNavigation<Ship> factory = MagicFactory.For<Ship>()
            .One()
            .With(x => x.Name = "Mary");
        factory
            .Do(saveShipName)
            .Build();

        shipName.Should().Be("Mary");
    }
    
    [Fact]
    public void should_execute_callback_through_ICustomizeOneBuildManyWithNavigation()
    {
        string shipName = null;
        
        Action<Ship> saveShipName = ship => shipName = ship.Name;
        ICustomizeOneBuildManyWithNavigation<Ship> factory = MagicFactory.For<Ship>()
            .One()
            .PlusOne()
            .With(x => x.Name = "Mary");
        factory
            .Do(saveShipName)
            .Build()
            .ToArray();

        shipName.Should().Be("Mary");
    }

    [Fact]
    public void should_execute_callback_in_a_customized_factory()
    {
        string shipName = null;
        
        Action<Ship> saveShipName = ship => shipName = ship.Name;
        new ShipFactory("Sauron", saveShipName).Build();
        
        shipName.Should().Be("Sauron");
    }

    private class ShipFactory : MagicFactory<Ship>
    {
        private readonly string _shipName;
        private readonly Action<Ship> _callback;

        public ShipFactory(string shipName, Action<Ship> callback)
        {
            _callback = callback;
            _shipName = shipName;
        }

        protected override void Customize(ICustomizeFactoryOptions<Ship> customization)
        {
            customization
                .Set(x => x.Name = _shipName)
                .Do(_callback);
        }
    }

    private class Ship
    {
        public string Name { get; set; }
    }
}