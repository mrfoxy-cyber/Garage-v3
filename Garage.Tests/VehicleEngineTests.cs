using Garage.Engines;
using Garage.Engines.Specifications;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;
using Xunit;

namespace Garage.Tests;

public sealed class VehicleEngineTests
{
    public static IEnumerable<object[]> VehicleTypes()
    {
        yield return ["Car"];
        yield return ["Motorcycle"];
        yield return ["Bus"];
        yield return ["Airplane"];
        yield return ["Boat"];
        yield return ["Truck"];
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void AddEngine_ExistingEnginePresent_PreservesOriginalAndAddsNewEngine(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);
        Engine added = EngineFixtures.CreateEngine(serialNumber: "ADDED123");

        // Act
        specification.AddEngine(added);

        // Assert
        Assert.Equal(2, specification.Engines.Count);
        Assert.Same(original, specification.FindEngine(original.SerialNumber));
        Assert.Same(added, specification.FindEngine("ADDED123"));
        Assert.Equal(1, specification.FindEngineIndex("ADDED123"));
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void RemoveEngine_TwoEnginesPresent_RemovesOnlySelectedEngine(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);
        Engine remaining = EngineFixtures.CreateEngine(serialNumber: "KEEP123");
        specification.AddEngine(remaining);

        // Act
        bool removed = specification.RemoveEngine(original);

        // Assert
        Assert.True(removed);
        Assert.Same(remaining, Assert.Single(specification.Engines));
        Assert.Null(specification.FindEngine(original.SerialNumber));
        Assert.Equal(-1, specification.FindEngineIndex(original.SerialNumber));

        Assert.False(specification.RemoveEngine(original));
        Assert.Same(remaining, Assert.Single(specification.Engines));
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void AddEngine_CapacityReached_ThrowsAndLeavesExistingEnginesUnchanged(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);

        for (int index = 1; index < VehicleSpecification.MaximumNumberOfEngines; index++)
            specification.AddEngine(EngineFixtures.CreateEngine(serialNumber: $"LIMIT-{index}"));

        Engine[] before = specification.Engines.ToArray();
        Engine extra = EngineFixtures.CreateEngine(serialNumber: "EXTRA");

        // Act
        Action act = () => specification.AddEngine(extra);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
        Assert.Equal(VehicleSpecification.MaximumNumberOfEngines, specification.Engines.Count);
        Assert.Equal(before, specification.Engines);
        Assert.Same(original, specification.FindEngine(original.SerialNumber));
        Assert.Null(specification.FindEngine("EXTRA"));
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void FindEngine_SerialHasDifferentCase_ReturnsSameEngineAndIndex(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);
        string serialNumber = original.SerialNumber.ToLowerInvariant();

        // Act
        Engine? found = specification.FindEngine(serialNumber);
        int index = specification.FindEngineIndex(serialNumber);

        // Assert
        Assert.Same(original, found);
        Assert.Equal(0, index);
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void RemoveEngine_LastEngineRemoved_LeavesEmptyCollectionAndNoLookupMatch(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);

        // Act
        bool removed = specification.RemoveEngine(original);

        // Assert
        Assert.True(removed);
        Assert.Empty(specification.Engines);
        Assert.Null(specification.FindEngine(original.SerialNumber));
        Assert.Equal(-1, specification.FindEngineIndex(original.SerialNumber));
        Assert.False(specification.RemoveEngine(original));
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void Engines_ReturnedListCleared_DoesNotChangeStoredEngines(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine original = Assert.Single(specification.Engines);
        var returned = specification.Engines;

        // Act
        returned.Clear();

        // Assert
        Assert.Same(original, Assert.Single(specification.Engines));
    }

    [Theory]
    [MemberData(nameof(VehicleTypes))]
    public void AddEngine_DetailedEngineProvided_PreservesItsIdentityAndSpecification(string vehicleType)
    {
        // Arrange
        VehicleSpecification specification = CreateSpecification(vehicleType);
        Engine added = EngineFixtures.CreateEngine(
            serialNumber: "DETAIL123",
            model: "Test engine",
            fuelType: FuelType.Gasoline,
            horsePower: 240,
            displacementCc: 2000,
            cylinderCount: 4);

        // Act
        specification.AddEngine(added);
        Engine? stored = specification.FindEngine("DETAIL123");

        // Assert
        Assert.NotNull(stored);
        Assert.Same(added, stored);
        Assert.Equal("DETAIL123", stored.SerialNumber);
        Assert.Equal("Test engine", stored.Model);
        Assert.Equal(FuelType.Gasoline, stored.FuelType);

        var details = Assert.IsAssignableFrom<PistonEngineSpecification>(stored.SpecificationDetails);
        Assert.Equal(240, details.HorsePower);
        Assert.Equal(2000, details.DisplacementCc);
        Assert.Equal(4, details.CylinderCount);
        Assert.Equal(500d, stored.CylinderVolumeCc);
    }

    private static VehicleSpecification CreateSpecification(string vehicleType)
    {
        Vehicle vehicle = vehicleType switch
        {
            "Car" => VehicleFixtures.CreateCar(FixtureKind.Full),
            "Motorcycle" => VehicleFixtures.CreateMotorcycle(FixtureKind.Full),
            "Bus" => VehicleFixtures.CreateBus(FixtureKind.Full),
            "Airplane" => VehicleFixtures.CreateAirplane(FixtureKind.Full),
            "Boat" => VehicleFixtures.CreateBoat(FixtureKind.Full),
            "Truck" => VehicleFixtures.CreateTruck(FixtureKind.Full),
            _ => throw new ArgumentException("Unsupported test vehicle type.", nameof(vehicleType))
        };

        return Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);
    }
}
