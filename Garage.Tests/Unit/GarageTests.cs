using Garage.Tests.Fixtures;
using Garage_V3;
using Garage_V3.results;
using System;
using System.Collections.Generic;
using System.Text;
using Vehicles.Interfaces;

namespace Garage.Tests.Unit;

public class GarageTests
{
    // Constructor

    [Fact]
    public void Constructor_ValidArguments_CreatesEmptyGarage()
    {
        // Arrange                      //Act
        Garage<IVehicle> garage = new Garage<IVehicle>("Test Garage", 3);

        // Act
        Assert.Equal("Test Garage", garage.GarageName);
        Assert.Equal(3, garage.Capacity);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(3, garage.AvailableSpaces);
        Assert.Empty(garage.Vehicles);
        Assert.Empty(garage);
    }

    [Fact]
    public void Constructor_NameWithSpaces_TrimsName()
    {
        // Arrange                      //Act
        Garage<IVehicle> garage = new Garage<IVehicle>("       Test Garage          ", 3);

        // Act
        Assert.Equal("Test Garage", garage.GarageName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ThrowsArgumentException(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Garage<IVehicle>(name!, 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100_001)]
    public void Constructor_InvalidCapacity_ThrowsArgumentOutOfRangeException(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Garage<IVehicle>("Test Garage", capacity));
    }

    // Available spots

    [Fact]
    public void ParkVehicle_EmptyGarage_AssignsFirstSpot()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 3);
        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal);

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.True(result.Success, result.Message);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Equal(1, parked.SpotNumber);
        Assert.Same(vehicle, parked.Vehicle);
    }

    [Fact]
    public void FindAvailableSpot_GapBetweenVehicles_ReturnsFirstFreeSpot()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 8);


        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal);
        IVehicle vehicle1 = VehicleFixtures.CreateCar(FixtureKind.Minimal);
        IVehicle vehicle2 = VehicleFixtures.CreateBus(FixtureKind.Minimal);
        IVehicle vehicle3 = VehicleFixtures.CreateTruck(FixtureKind.Minimal);
        IVehicle vehicle4 = VehicleFixtures.CreateAirplane(FixtureKind.Minimal);
        IVehicle vehicle5 = VehicleFixtures.CreateBoat(FixtureKind.Minimal);

        // Act
        var result = garage.ParkVehicle(vehicle,1);
        var result1 = garage.ParkVehicle(vehicle1, 2);
        var result2 = garage.ParkVehicle(vehicle2, 3);
        var result3 = garage.ParkVehicle(vehicle3, 6);
        var result4 = garage.ParkVehicle(vehicle4, 8);

        var result5 = garage.ParkVehicle(vehicle5);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.True(result1.Success, result1.Message);
        Assert.True(result2.Success, result2.Message);
        Assert.True(result3.Success, result3.Message);
        Assert.True(result4.Success, result4.Message);
        Assert.True(result5.Success, result5.Message);

        var parked = Assert.Single(garage.Vehicles, entry => ReferenceEquals(entry.Vehicle, vehicle5));

        Assert.Equal(4, parked.SpotNumber);

    }

    [Fact]
    public void ParkVehicle_FullGarage_ReturnsFailureAndKeepsExistingVehicles()
    {

        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 5);


        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal);
        IVehicle vehicle1 = VehicleFixtures.CreateCar(FixtureKind.Minimal);
        IVehicle vehicle2 = VehicleFixtures.CreateBus(FixtureKind.Minimal);
        IVehicle vehicle3 = VehicleFixtures.CreateTruck(FixtureKind.Minimal);
        IVehicle vehicle4 = VehicleFixtures.CreateAirplane(FixtureKind.Minimal);
        IVehicle vehicle5 = VehicleFixtures.CreateBoat(FixtureKind.Minimal);

        // Act
        var result = garage.ParkVehicle(vehicle);
        var result1 = garage.ParkVehicle(vehicle1);
        var result2 = garage.ParkVehicle(vehicle2);
        var result3 = garage.ParkVehicle(vehicle3);
        var result4 = garage.ParkVehicle(vehicle4);

        var result5 = garage.ParkVehicle(vehicle5);
        // Assert
        Assert.True(result.Success, result.Message);
        Assert.True(result1.Success, result1.Message);
        Assert.True(result2.Success, result2.Message);
        Assert.True(result3.Success, result3.Message);
        Assert.True(result4.Success, result4.Message);

        Assert.False(result5.Success);
        Assert.Equal(OperationError.GarageFull, result5.Error);

        Assert.Equal(5, garage.OccupiedSpaces);
        Assert.Equal(0, garage.AvailableSpaces);

        // The original vehicles remain in their assigned spots.
        Assert.Collection(garage.Vehicles,
            entry => { Assert.Equal(1, entry.SpotNumber); Assert.Same(vehicle, entry.Vehicle); },
            entry => { Assert.Equal(2, entry.SpotNumber); Assert.Same(vehicle1, entry.Vehicle); },
            entry => { Assert.Equal(3, entry.SpotNumber); Assert.Same(vehicle2, entry.Vehicle); },
            entry => { Assert.Equal(4, entry.SpotNumber); Assert.Same(vehicle3, entry.Vehicle); },
            entry => { Assert.Equal(5, entry.SpotNumber); Assert.Same(vehicle4, entry.Vehicle); });
    }

    // Parking

    [Fact]
    public void ParkVehicle_NoSpotSpecified_UsesFirstAvailableSpot()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 3);
        IVehicle existingVehicle = VehicleFixtures.CreateCar(FixtureKind.Minimal);
        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal);

        var setupResult = garage.ParkVehicle(existingVehicle, 1);
        Assert.True(setupResult.Success, setupResult.Message);

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.True(result.Success, result.Message);

        var parked = Assert.Single(garage.Vehicles, entry => ReferenceEquals(entry.Vehicle, vehicle));
        Assert.Equal(2, parked.SpotNumber);
    }

    [Fact]
    public void ParkVehicle_FreeSpotSpecified_UsesRequestedSpot()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 5);
        IVehicle vehicle = VehicleFixtures.CreateBus(FixtureKind.Minimal);

        // Act
        var result = garage.ParkVehicle(vehicle, 4);

        // Assert
        Assert.True(result.Success, result.Message);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Equal(4, parked.SpotNumber);
        Assert.Same(vehicle, parked.Vehicle);
    }

    [Fact]
    public void ParkVehicle_Success_UpdatesVehiclesAndSpaceCounts()
    {
        Garage<IVehicle> garage = new("Test Garage", 5);

        IVehicle vehicle = VehicleFixtures.CreateBus(FixtureKind.Minimal);

        int initialOccupied = garage.OccupiedSpaces;
        int availableBefore = garage.AvailableSpaces;

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.True(result.Success, result.Message);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Same(vehicle, parked.Vehicle);
        Assert.Equal(initialOccupied + 1, garage.OccupiedSpaces);
        Assert.Equal(availableBefore - 1, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_NullVehicle_ReturnsValidationFailure()
    {
        Garage<IVehicle> garage = new("Test Garage", 5);

        IVehicle vehicle = null;

        int initialOccupied = garage.OccupiedSpaces;
        int availableBefore = garage.AvailableSpaces;

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.False(result.Success, result.Message);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Equal(initialOccupied, garage.OccupiedSpaces);
        Assert.Equal(availableBefore, garage.AvailableSpaces);  
    }

    [Fact(Skip = " Vehcile luckily doesnt allow null reg, so skip this test for now")]  
    public void ParkVehicle_BlankRegistration_ReturnsValidationFailure()
    {
    }

    [Fact]
    public void ParkVehicle_DuplicateRegistrationIgnoringCase_ReturnsFailureWithoutChanges()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 3);
        IVehicle original = VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "ABC123");
        IVehicle duplicate = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: "abc123");

        var setupResult = garage.ParkVehicle(original, 2);
        Assert.True(setupResult.Success, setupResult.Message);

        // Act
        var result = garage.ParkVehicle(duplicate);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Same(original, parked.Vehicle);
        Assert.Equal(2, parked.SpotNumber);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4)]
    public void ParkVehicle_SpotOutsideCapacityThree_ReturnsInvalidSpot(int spotNumber)
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 3);
        IVehicle vehicle = VehicleFixtures.CreateCar(FixtureKind.Minimal);

        // Act
        var result = garage.ParkVehicle(vehicle, spotNumber);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.InvalidSpot, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(3, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_OccupiedSpot_ReturnsFailureWithoutChanges()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 3);
        IVehicle original = VehicleFixtures.CreateCar(FixtureKind.Minimal);
        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal);

        var setupResult = garage.ParkVehicle(original, 2);
        Assert.True(setupResult.Success, setupResult.Message);

        // Act
        var result = garage.ParkVehicle(vehicle, 2);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.SpotOccupied, result.Error);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Same(original, parked.Vehicle);
        Assert.Equal(2, parked.SpotNumber);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_FullGarage_ReturnsFailureWithoutChanges()
    {
        // Arrange
        Garage<IVehicle> garage = new("Test Garage", 1);
        IVehicle original = VehicleFixtures.CreateBus(FixtureKind.Minimal);
        IVehicle vehicle = VehicleFixtures.CreateBoat(FixtureKind.Minimal);

        var setupResult = garage.ParkVehicle(original);
        Assert.True(setupResult.Success, setupResult.Message);

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);

        var parked = Assert.Single(garage.Vehicles);
        Assert.Same(original, parked.Vehicle);
        Assert.Equal(1, parked.SpotNumber);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(0, garage.AvailableSpaces);
    }

    // Removal
    [Fact]
    public void RemoveVehicle_ExistingRegistration_RemovesOnlyMatchingVehicleAndFreesSpace()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        var garage = fixture.Garage;
        var target = fixture.ExpectedVehicles[0];

        // Act
        var result = garage.RemoveVehicle(target.Vehicle.RegistrationNumber);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.DoesNotContain(garage.Vehicles, entry => ReferenceEquals(entry.Vehicle, target.Vehicle));
        Assert.Equal(fixture.ExpectedOccupiedSpaces - 1, garage.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces + 1, garage.AvailableSpaces);

        foreach (var expected in fixture.ExpectedVehicles.Skip(1))
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Fact]
    public void RemoveVehicle_RegistrationWithDifferentCaseAndSpaces_RemovesVehicle()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        var garage = fixture.Garage;
        var target = fixture.ExpectedVehicles[0];
        string registration = $"  {target.Vehicle.RegistrationNumber.ToLowerInvariant()}  ";

        // Act
        var result = garage.RemoveVehicle(registration);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.DoesNotContain(garage.Vehicles, entry => ReferenceEquals(entry.Vehicle, target.Vehicle));
        Assert.Equal(fixture.ExpectedOccupiedSpaces - 1, garage.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces + 1, garage.AvailableSpaces);
    }

    [Fact]
    public void RemoveVehicle_UnknownRegistration_ReturnsFailureWithoutChanges()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        var garage = fixture.Garage;

        // Act
        var result = garage.RemoveVehicle("UNKNOWN");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, garage.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, garage.AvailableSpaces);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RemoveVehicle_InvalidRegistration_ReturnsValidationFailure(string? registrationNumber)
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        var garage = fixture.Garage;

        // Act
        var result = garage.RemoveVehicle(registrationNumber!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, garage.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, garage.AvailableSpaces);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    // Initialization

    [Fact]
    public void InitializeVehicles_ValidVehicles_ParksVehiclesInSequentialSpots()
    {
        // Arrange
        var vehicles = VehicleFixtures.CreateAll(FixtureKind.Minimal);
        var garage = GarageFixtures.CreateGarage(capacity: vehicles.Count + 2);

        // Act
        var result = garage.InitializeVehicles(vehicles);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(vehicles.Count, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
        Assert.Equal(vehicles.Count, garage.Vehicles.Count);

        for (int index = 0; index < vehicles.Count; index++)
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == index + 1);
            Assert.Same(vehicles[index], parked.Vehicle);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InitializeVehicles_EmptyOrExactlyCapacity_Succeeds(bool fillGarage)
    {
        // Arrange
        var vehicles = fillGarage
            ? VehicleFixtures.CreateAll(FixtureKind.Minimal)
            : Array.Empty<IVehicle>();

        int capacity = fillGarage ? vehicles.Count : 3;
        var garage = GarageFixtures.CreateGarage(capacity: capacity);

        // Act
        var result = garage.InitializeVehicles(vehicles);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(vehicles.Count, garage.OccupiedSpaces);
        Assert.Equal(capacity - vehicles.Count, garage.AvailableSpaces);
        Assert.Equal(vehicles.Count, garage.Vehicles.Count);

        for (int index = 0; index < vehicles.Count; index++)
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == index + 1);
            Assert.Same(vehicles[index], parked.Vehicle);
        }
    }

    [Fact]
    public void InitializeVehicles_NullCollection_ReturnsValidationFailure()
    {
        // Arrange
        var garage = GarageFixtures.CreateGarage();

        // Act
        var result = garage.InitializeVehicles(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(garage.Capacity, garage.AvailableSpaces);
    }

    [Fact]
    public void InitializeVehicles_NonEmptyGarage_ReturnsFailureWithoutChanges()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles(capacity: 10);
        var garage = fixture.Garage;
        IVehicle additionalVehicle = VehicleFixtures.CreateCar(
            FixtureKind.Minimal, registrationNumber: "NEW123");

        // Act
        var result = garage.InitializeVehicles([additionalVehicle]);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, garage.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, garage.AvailableSpaces);
        Assert.Equal(fixture.ExpectedVehicles.Count, garage.Vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(garage.Vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Fact]
    public void InitializeVehicles_TooManyVehicles_ReturnsFailureAndLeavesGarageEmpty()
    {
        // Arrange
        var vehicles = VehicleFixtures.CreateAll(FixtureKind.Minimal);
        var garage = GarageFixtures.CreateGarage(capacity: vehicles.Count - 1);

        // Act
        var result = garage.InitializeVehicles(vehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(garage.Capacity, garage.AvailableSpaces);
    }

    [Fact]
    public void InitializeVehicles_NullVehicle_ReturnsFailureAndLeavesGarageEmpty()
    {
        // Arrange
        var garage = GarageFixtures.CreateGarage(capacity: 3);
        IVehicle[] vehicles =
        [
            VehicleFixtures.CreateCar(FixtureKind.Minimal),
        null!,
        VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal)
        ];

        // Act
        var result = garage.InitializeVehicles(vehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(garage.Capacity, garage.AvailableSpaces);
    }

    [Fact(Skip = "Requires an IVehicle test double that permits blank registration.")]
    public void InitializeVehicles_BlankRegistration_ReturnsFailureAndLeavesGarageEmpty()
    {
    }

    [Fact]
    public void InitializeVehicles_DuplicateRegistrationsIgnoringCase_ReturnsFailureAndLeavesGarageEmpty()
    {
        // Arrange
        var garage = GarageFixtures.CreateGarage(capacity: 5);
        IVehicle[] vehicles =
        [
            VehicleFixtures.CreateBus(FixtureKind.Minimal),
        VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "ABC123"),
        VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: "abc123")
        ];

        // Act
        var result = garage.InitializeVehicles(vehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(garage.Capacity, garage.AvailableSpaces);
    }

    // Enumeration

    [Fact]
    public void GetEnumerator_EmptyGarage_ReturnsNoVehicles()
    {
        // Arrange
        var garage = GarageFixtures.CreateGarage();

        // Act
        using var enumerator = garage.GetEnumerator();
        bool hasVehicle = enumerator.MoveNext();

        // Assert
        Assert.False(hasVehicle);
    }

    [Fact]
    public void GetEnumerator_ParkedVehicles_ReturnsEachVehicleOnce()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        List<IVehicle> actual = [];

        // Act
        foreach (IVehicle vehicle in fixture.Garage)
            actual.Add(vehicle);

        // Assert
        Assert.Equal(fixture.ExpectedVehicles.Count, actual.Count);

        foreach (var expected in fixture.ExpectedVehicles)
            Assert.Single(actual, vehicle => ReferenceEquals(vehicle, expected.Vehicle));
    }

    [Fact]
    public void NonGenericGetEnumerator_ParkedVehicles_ReturnsEachVehicleOnce()
    {
        // Arrange
        var fixture = GarageFixtures.CreateGarageWithVehicles();
        System.Collections.IEnumerable enumerable = fixture.Garage;
        List<object> actual = [];

        // Act
        foreach (object vehicle in enumerable)
            actual.Add(vehicle);

        // Assert
        Assert.Equal(fixture.ExpectedVehicles.Count, actual.Count);

        foreach (var expected in fixture.ExpectedVehicles)
            Assert.Single(actual, vehicle => ReferenceEquals(vehicle, expected.Vehicle));
    }
}
