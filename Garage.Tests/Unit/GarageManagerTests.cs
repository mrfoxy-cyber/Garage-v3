using System.Text.Json;
using Garage.Manager.Interfaces;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Garage_V3.queries;
using Garage_V3.results;
using Vehicles.Interfaces;
using Xunit;

namespace Garage.Tests.Unit;

public class GarageManagerTests
{
    // CreateGarage

    [Fact]
    public void CreateGarage_ValidArguments_ReturnsIdAndRegistersGarage()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();

        // Act
        var (id, result) = manager.CreateGarage("New garage", 5);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.False(string.IsNullOrWhiteSpace(id));
        var (garages, listResult) = manager.GetGarages();
        Assert.True(listResult.Success, listResult.Message);
        Assert.Equal(id, Assert.Single(garages).Id);
        AssertGarageInfo(manager, id!, "New garage", 5, 0);
    }

    [Fact]
    public void CreateGarage_MultipleGarages_ReturnsDistinctIds()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();

        // Act
        var (firstId, firstResult) = manager.CreateGarage("First garage", 3);
        var (secondId, secondResult) = manager.CreateGarage("Second garage", 7);

        // Assert
        Assert.True(firstResult.Success, firstResult.Message);
        Assert.True(secondResult.Success, secondResult.Message);
        Assert.False(string.IsNullOrWhiteSpace(firstId));
        Assert.False(string.IsNullOrWhiteSpace(secondId));
        Assert.NotEqual(firstId, secondId);
        var (garages, result) = manager.GetGarages();
        Assert.True(result.Success, result.Message);
        Assert.Equal(2, garages.Count);
        Assert.Single(garages, garage => garage.Id == firstId);
        Assert.Single(garages, garage => garage.Id == secondId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateGarage_InvalidName_ReturnsValidationFailure(string? name)
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (id, result) = fixture.Manager.CreateGarage(name!, 5);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_NameWithSpaces_StoresTrimmedName()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();

        // Act
        var (id, result) = manager.CreateGarage("  Trimmed garage  ", 5);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(id);
        AssertGarageInfo(manager, id, "Trimmed garage", 5, 0);
    }

    [Fact]
    public void CreateGarage_DuplicateNameIgnoringCaseAndSpaces_ReturnsFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var original = fixture.GetGarage(ManagerGarageKind.Car);

        // Act
        var (id, result) = fixture.Manager.CreateGarage($"  {original.ExpectedName.ToUpperInvariant()}  ", 20);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageAlreadyExists, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(100_001)]
    public void CreateGarage_InvalidCapacity_DoesNotRegisterGarage(int capacity)
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Invalid capacity", capacity);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_InitialVehicles_PopulatesGarageAndRegistersLocations()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();
        var initialVehicles = VehicleFixtures.CreateAll(FixtureKind.Full);
        int capacity = initialVehicles.Count + 2;

        // Act
        var (id, result) = manager.CreateGarage("Initialized garage", capacity, initialVehicles);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(id);
        AssertGarageInfo(manager, id, "Initialized garage", capacity, initialVehicles.Count);
        var (vehicles, vehiclesResult) = manager.GetVehicles(id);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(initialVehicles.Count, vehicles.Count);
        for (int index = 0; index < initialVehicles.Count; index++)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == index + 1);
            Assert.Same(initialVehicles[index], parked.Vehicle);
            AssertLocation(manager, initialVehicles[index], id, "Initialized garage", index + 1);
        }
    }

    [Fact]
    public void CreateGarage_InvalidInitialVehicles_DoesNotRegisterGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle valid = NewVehicle();
        IVehicle[] initialVehicles = [valid, null!];

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Invalid vehicles", 5, initialVehicles);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertVehicleNotFound(fixture.Manager, valid.RegistrationNumber);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_InitialVehiclesExceedCapacity_DoesNotRegisterGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle car = NewVehicle();
        IVehicle motorcycle = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: "NEW456");

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Too small", 1, [car, motorcycle]);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);
        AssertVehicleNotFound(fixture.Manager, car.RegistrationNumber);
        AssertVehicleNotFound(fixture.Manager, motorcycle.RegistrationNumber);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_DuplicateInitialRegistrations_DoesNotRegisterGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle car = NewVehicle();
        IVehicle duplicate = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: car.RegistrationNumber.ToLowerInvariant());

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Duplicate vehicles", 5, [car, duplicate]);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        AssertVehicleNotFound(fixture.Manager, car.RegistrationNumber);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_RegistrationAlreadyInAnotherGarage_LeavesExistingGaragesUnchanged()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle fresh = NewVehicle();
        IVehicle existing = fixture.GetGarage(ManagerGarageKind.Car).ExpectedVehicles[0].Vehicle;

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Conflicting garage", 5, [fresh, existing]);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        AssertVehicleNotFound(fixture.Manager, fresh.RegistrationNumber);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void CreateGarage_FailedInitialization_DoesNotReserveNameOrVehicleRegistrations()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle car = NewVehicle();
        IVehicle duplicate = VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: car.RegistrationNumber);
        var (failedId, failedResult) = fixture.Manager.CreateGarage("Retry garage", 2, [car, duplicate]);
        Assert.Null(failedId);
        Assert.False(failedResult.Success);
        Assert.Equal(OperationError.DuplicateRegistration, failedResult.Error);
        AssertManagerUnchanged(fixture);

        // Act
        var (id, result) = fixture.Manager.CreateGarage("Retry garage", 2, [car]);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(id);
        AssertLocation(fixture.Manager, car, id, "Retry garage", 1);
        AssertGarageInfo(fixture.Manager, id, "Retry garage", 2, 1);
        var (garages, listResult) = fixture.Manager.GetGarages();
        Assert.True(listResult.Success, listResult.Message);
        Assert.Equal(fixture.ExpectedGarageCount + 1, garages.Count);
    }

    // GetGarages

    [Fact]
    public void GetGarages_NoGarages_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();

        // Act
        var (garages, result) = manager.GetGarages();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(garages);
    }

    [Fact]
    public void GetGarages_MultipleGarages_ReturnsCorrectIdsAndInformation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (garages, result) = fixture.Manager.GetGarages();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(fixture.ExpectedGarageCount, garages.Count);
        foreach (var expected in fixture.Garages)
        {
            var actual = Assert.Single(garages, garage => garage.Id == expected.GarageId);
            Assert.Equal(new Garage.Manager.Queries.ManagedGarageInfo(expected.GarageId,
                new GarageInfo(expected.ExpectedName, expected.ExpectedCapacity, expected.ExpectedOccupiedSpaces)), actual);
        }
    }

    [Fact]
    public void GetGarages_AfterParkingAndRemoval_ReturnsUpdatedOccupancy()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = NewVehicle();
        var parkResult = fixture.Manager.ParkVehicle(target.GarageId, vehicle);
        Assert.True(parkResult.Success, parkResult.Message);

        // Act and Assert — after parking.
        var (afterParking, parkingResult) = fixture.Manager.GetGarages();
        Assert.True(parkingResult.Success, parkingResult.Message);
        var parkedGarage = Assert.Single(afterParking, garage => garage.Id == target.GarageId);
        Assert.Equal(new Garage.Manager.Queries.ManagedGarageInfo(target.GarageId,
            new GarageInfo(target.ExpectedName, target.ExpectedCapacity, 1)), parkedGarage);

        var removeResult = fixture.Manager.RemoveVehicle(target.GarageId, vehicle.RegistrationNumber);
        Assert.True(removeResult.Success, removeResult.Message);

        // Act and Assert — after removal.
        var (afterRemoval, removalResult) = fixture.Manager.GetGarages();
        Assert.True(removalResult.Success, removalResult.Message);
        var emptyGarage = Assert.Single(afterRemoval, garage => garage.Id == target.GarageId);
        Assert.Equal(new Garage.Manager.Queries.ManagedGarageInfo(target.GarageId,
            new GarageInfo(target.ExpectedName, target.ExpectedCapacity, 0)), emptyGarage);
        AssertManagerUnchanged(fixture);
    }

    // GetGarageInfo

    [Fact]
    public void GetGarageInfo_ExistingId_ReturnsRequestedGarageInformation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        foreach (var expected in fixture.Garages)
            AssertGarageInfo(fixture.Manager, expected.GarageId, expected.ExpectedName, expected.ExpectedCapacity, expected.ExpectedOccupiedSpaces);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("unknown-id")]
    public void GetGarageInfo_InvalidId_ReturnsNullAndNoGarageFailure(string? garageId)
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (info, result) = fixture.Manager.GetGarageInfo(garageId!);

        // Assert
        Assert.Null(info);
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    // ParkVehicle

    [Fact]
    public void ParkVehicle_ExistingGarage_ParksVehicleAndRegistersLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = NewVehicle();

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, vehicle);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocation(fixture.Manager, vehicle, target.GarageId, target.ExpectedName, 1);
        var (vehicles, vehiclesResult) = fixture.Manager.GetVehicles(target.GarageId);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Same(vehicle, Assert.Single(vehicles).Vehicle);
        AssertGarageInfo(fixture.Manager, target.GarageId, target.ExpectedName, target.ExpectedCapacity, 1);
    }

    [Fact]
    public void ParkVehicle_RequestedSpot_RecordsCorrectSpot()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = NewVehicle();

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, vehicle, 4);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocation(fixture.Manager, vehicle, target.GarageId, target.ExpectedName, 4);
        var (vehicles, vehiclesResult) = fixture.Manager.GetVehicles(target.GarageId);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        var parked = Assert.Single(vehicles);
        Assert.Equal(4, parked.SpotNumber);
        Assert.Same(vehicle, parked.Vehicle);
    }

    [Fact]
    public void ParkVehicle_UnknownGarage_ReturnsNoGarageFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle vehicle = NewVehicle();

        // Act
        var result = fixture.Manager.ParkVehicle("unknown-id", vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_NullVehicle_ReturnsValidationFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_BlankRegistration_ReturnsValidationFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = new InvalidRegistrationVehicle { RegistrationNumber = "   " };

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_RegistrationAlreadyInSameGarage_ReturnsDuplicateFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Car);
        string registration = target.ExpectedVehicles[0].Vehicle.RegistrationNumber;
        IVehicle duplicate = VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: registration);

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, duplicate);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_RegistrationAlreadyInAnotherGarage_ReturnsFailureNamingThatGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var original = fixture.GetGarage(ManagerGarageKind.Car);
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = original.ExpectedVehicles[0].Vehicle;

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        Assert.Contains(original.ExpectedName, result.Message);
        Assert.Contains(vehicle.RegistrationNumber, result.Message);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_DuplicateRegistrationWithDifferentCase_ReturnsDuplicateFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var original = fixture.GetGarage(ManagerGarageKind.Car);
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        string registration = original.ExpectedVehicles[0].Vehicle.RegistrationNumber.ToLowerInvariant();
        IVehicle duplicate = VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: registration);

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, duplicate);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void ParkVehicle_FailedParking_DoesNotRegisterVehicleLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var occupiedGarage = fixture.GetGarage(ManagerGarageKind.Car);
        var emptyGarage = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = NewVehicle();

        // Act
        var result = fixture.Manager.ParkVehicle(occupiedGarage.GarageId, vehicle, 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.SpotOccupied, result.Error);
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);
        AssertManagerUnchanged(fixture);

        var retry = fixture.Manager.ParkVehicle(emptyGarage.GarageId, vehicle);
        Assert.True(retry.Success, retry.Message);
        AssertLocation(fixture.Manager, vehicle, emptyGarage.GarageId, emptyGarage.ExpectedName, 1);
    }

    [Fact]
    public void ParkVehicle_OneGarageFull_AllowsParkingInAnotherGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var emptyGarage = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle original = VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "ONE123");
        var (fullId, creationResult) = fixture.Manager.CreateGarage("Full garage", 1, [original]);
        Assert.True(creationResult.Success, creationResult.Message);
        Assert.NotNull(fullId);
        IVehicle vehicle = NewVehicle();

        // Act
        var failed = fixture.Manager.ParkVehicle(fullId, vehicle);
        var success = fixture.Manager.ParkVehicle(emptyGarage.GarageId, vehicle);

        // Assert
        Assert.False(failed.Success);
        Assert.Equal(OperationError.GarageFull, failed.Error);
        Assert.True(success.Success, success.Message);
        AssertLocation(fixture.Manager, original, fullId, "Full garage", 1);
        AssertLocation(fixture.Manager, vehicle, emptyGarage.GarageId, emptyGarage.ExpectedName, 1);
    }

    // RemoveVehicle

    [Fact]
    public void RemoveVehicle_ExistingVehicle_RemovesVehicleAndLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Mixed);
        IVehicle vehicle = target.ExpectedVehicles[0].Vehicle;

        // Act
        var result = fixture.Manager.RemoveVehicle(target.GarageId, vehicle.RegistrationNumber);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertManagerUnchanged(fixture, removed: vehicle);
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);
    }

    [Fact]
    public void RemoveVehicle_RegistrationWithDifferentCaseAndSpaces_RemovesVehicle()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Mixed);
        IVehicle vehicle = target.ExpectedVehicles[0].Vehicle;

        // Act
        var result = fixture.Manager.RemoveVehicle(target.GarageId, $"  {vehicle.RegistrationNumber.ToLowerInvariant()}  ");

        // Assert
        Assert.True(result.Success, result.Message);
        AssertManagerUnchanged(fixture, removed: vehicle);
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);
    }

    [Fact]
    public void RemoveVehicle_UnknownGarage_ReturnsNoGarageFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        IVehicle vehicle = fixture.GetGarage(ManagerGarageKind.Car).ExpectedVehicles[0].Vehicle;

        // Act
        var result = fixture.Manager.RemoveVehicle("unknown-id", vehicle.RegistrationNumber);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RemoveVehicle_InvalidRegistration_ReturnsValidationFailure(string? registrationNumber)
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Car);

        // Act
        var result = fixture.Manager.RemoveVehicle(target.GarageId, registrationNumber!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void RemoveVehicle_UnknownRegistration_ReturnsVehicleNotFound()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Car);

        // Act
        var result = fixture.Manager.RemoveVehicle(target.GarageId, "UNKNOWN");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void RemoveVehicle_WrongGarage_KeepsVehicleAndOriginalLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var original = fixture.GetGarage(ManagerGarageKind.Car);
        var wrongGarage = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = original.ExpectedVehicles[0].Vehicle;

        // Act
        var result = fixture.Manager.RemoveVehicle(wrongGarage.GarageId, vehicle.RegistrationNumber);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void RemoveVehicle_Success_AllowsRegistrationToBeParkedInAnotherGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var original = fixture.GetGarage(ManagerGarageKind.Car);
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = original.ExpectedVehicles[0].Vehicle;
        var removal = fixture.Manager.RemoveVehicle(original.GarageId, vehicle.RegistrationNumber);
        Assert.True(removal.Success, removal.Message);
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);

        // Act
        var result = fixture.Manager.ParkVehicle(target.GarageId, vehicle, 3);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocation(fixture.Manager, vehicle, target.GarageId, target.ExpectedName, 3);
        var (oldVehicles, oldResult) = fixture.Manager.GetVehicles(original.GarageId);
        Assert.True(oldResult.Success, oldResult.Message);
        Assert.Empty(oldVehicles);
        AssertGarageInfo(fixture.Manager, original.GarageId, original.ExpectedName, original.ExpectedCapacity, 0);
        AssertGarageInfo(fixture.Manager, target.GarageId, target.ExpectedName, target.ExpectedCapacity, 1);
    }

    // GetVehicles

    [Fact]
    public void GetVehicles_UnknownGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (vehicles, result) = fixture.Manager.GetVehicles("unknown-id");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void GetVehicles_EmptyGarage_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);

        // Act
        var (vehicles, result) = fixture.Manager.GetVehicles(target.GarageId);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void GetVehicles_MultipleGarages_ReturnsOnlyRequestedGarageVehicles()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        foreach (var expected in fixture.Garages)
        {
            var (vehicles, result) = fixture.Manager.GetVehicles(expected.GarageId);
            Assert.True(result.Success, result.Message);
            AssertParkedVehicles(vehicles, expected.ExpectedVehicles);
        }
    }

    // GetVehicleTypeCounts

    [Fact]
    public void GetVehicleTypeCounts_UnknownGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (counts, result) = fixture.Manager.GetVehicleTypeCounts("unknown-id");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(counts);
    }

    [Fact]
    public void GetVehicleTypeCounts_EmptyGarage_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);

        // Act
        var (counts, result) = fixture.Manager.GetVehicleTypeCounts(target.GarageId);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(counts);
    }

    [Fact]
    public void GetVehicleTypeCounts_MultipleGarages_CountsOnlyRequestedGarageVehicles()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        foreach (var expected in fixture.Garages)
        {
            var (counts, result) = fixture.Manager.GetVehicleTypeCounts(expected.GarageId);
            Assert.True(result.Success, result.Message);
            Assert.Equal(expected.ExpectedTypeCounts.Count, counts.Count);
            Assert.Equal(expected.ExpectedOccupiedSpaces, counts.Sum(entry => entry.Count));
            foreach (var type in expected.ExpectedTypeCounts)
            {
                var actual = Assert.Single(counts, entry => entry.VehicleType == type.Key);
                Assert.Equal(type.Value, actual.Count);
            }
        }
    }

    // SearchVehicles — within one garage

    [Fact]
    public void SearchVehicles_UnknownGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (vehicles, result) = fixture.Manager.SearchVehicles("unknown-id", new VehicleSearchCriteria());

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_NullCriteria_ReturnsValidationFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Mixed);

        // Act
        var (vehicles, result) = fixture.Manager.SearchVehicles(target.GarageId, null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_MatchesInMultipleGarages_ReturnsOnlyRequestedGarageMatches()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Mixed);
        var expected = target.ExpectedVehicles.Where(entry => entry.Vehicle is Car).ToList();
        VehicleSearchCriteria criteria = new() { VehicleType = nameof(Car) };

        // Act
        var (vehicles, result) = fixture.Manager.SearchVehicles(target.GarageId, criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertParkedVehicles(vehicles, expected);
        Assert.DoesNotContain(vehicles, entry => fixture.GetGarage(ManagerGarageKind.Car)
            .ExpectedVehicles.Any(other => ReferenceEquals(other.Vehicle, entry.Vehicle)));
    }

    [Fact]
    public void SearchVehicles_NoMatches_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages(FixtureKind.Full);
        var target = fixture.GetGarage(ManagerGarageKind.Mixed);
        VehicleSearchCriteria criteria = new() { Color = VehicleColor.Pink };

        // Act
        var (vehicles, result) = fixture.Manager.SearchVehicles(target.GarageId, criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    // FindVehicle — across all garages

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FindVehicle_InvalidRegistration_ReturnsNullAndValidationFailure(string? registrationNumber)
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (location, result) = fixture.Manager.FindVehicle(registrationNumber!);

        // Assert
        Assert.Null(location);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
    }

    [Fact]
    public void FindVehicle_UnknownRegistration_ReturnsNullAndVehicleNotFound()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        AssertVehicleNotFound(fixture.Manager, "UNKNOWN");
    }

    [Fact]
    public void FindVehicle_ParkedVehicle_ReturnsVehicleGarageIdGarageNameAndSpot()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Empty);
        IVehicle vehicle = NewVehicle();
        var setup = fixture.Manager.ParkVehicle(target.GarageId, vehicle, 4);
        Assert.True(setup.Success, setup.Message);

        // Act and Assert
        AssertLocation(fixture.Manager, vehicle, target.GarageId, target.ExpectedName, 4);
    }

    [Fact]
    public void FindVehicle_RegistrationWithDifferentCaseAndSpaces_ReturnsMatchingLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        foreach (var expected in fixture.ExpectedLocations)
        {
            var (location, result) = fixture.Manager.FindVehicle($"  {expected.Vehicle.RegistrationNumber.ToLowerInvariant()}  ");
            Assert.True(result.Success, result.Message);
            Assert.NotNull(location);
            Assert.Equal(expected.GarageId, location.GarageId);
            Assert.Equal(expected.GarageName, location.GarageName);
            Assert.Equal(expected.SpotNumber, location.SpotNumber);
            Assert.Same(expected.Vehicle, location.Vehicle);
        }
    }

    [Fact]
    public void FindVehicle_InitializedVehicle_ReturnsCorrectLocation()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act and Assert
        foreach (var expected in fixture.ExpectedLocations)
            AssertLocation(fixture.Manager, expected.Vehicle, expected.GarageId, expected.GarageName, expected.SpotNumber);
    }

    [Fact]
    public void FindVehicle_RemovedVehicle_ReturnsVehicleNotFound()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var target = fixture.GetGarage(ManagerGarageKind.Car);
        IVehicle vehicle = target.ExpectedVehicles[0].Vehicle;
        var removal = fixture.Manager.RemoveVehicle(target.GarageId, vehicle.RegistrationNumber);
        Assert.True(removal.Success, removal.Message);

        // Act and Assert
        AssertVehicleNotFound(fixture.Manager, vehicle.RegistrationNumber);
    }

    // SearchAllVehicles

    [Fact]
    public void SearchAllVehicles_NullCriteria_ReturnsEmptyCollectionAndValidationFailure()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (vehicles, result) = fixture.Manager.SearchAllVehicles(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchAllVehicles_NoGarages_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var manager = GarageManagerFixtures.CreateManager();

        // Act
        var (vehicles, result) = manager.SearchAllVehicles(new VehicleSearchCriteria());

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchAllVehicles_NoFilters_ReturnsVehiclesFromEveryGarage()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();

        // Act
        var (vehicles, result) = fixture.Manager.SearchAllVehicles(new VehicleSearchCriteria());

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocations(vehicles, fixture.ExpectedLocations);
    }

    [Fact]
    public void SearchAllVehicles_MatchingVehicles_ReturnsMatchesWithCorrectGarageAndSpot()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        var expected = fixture.ExpectedLocations.Where(entry => entry.Vehicle is Motorcycle).ToList();
        VehicleSearchCriteria criteria = new() { VehicleType = nameof(Motorcycle) };

        // Act
        var (vehicles, result) = fixture.Manager.SearchAllVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocations(vehicles, expected);
    }

    [Fact]
    public void SearchAllVehicles_CombinedFilters_ReturnsOnlyVehiclesMatchingAllCriteria()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages(FixtureKind.Full);
        var expected = fixture.ExpectedLocations.Where(entry => entry.Vehicle is Car).ToList();
        VehicleSearchCriteria criteria = new()
        {
            VehicleType = nameof(Car),
            Manufacturer = "Volvo",
            Model = "V70",
            Color = VehicleColor.Black,
            ManufacturingYear = 2020,
            NumberOfWheels = 4
        };

        // Act
        var (vehicles, result) = fixture.Manager.SearchAllVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertLocations(vehicles, expected);
    }

    [Fact]
    public void SearchAllVehicles_NoMatches_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages(FixtureKind.Full);
        VehicleSearchCriteria criteria = new() { Color = VehicleColor.Pink };

        // Act
        var (vehicles, result) = fixture.Manager.SearchAllVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    // LoadGarage — each file is created and deleted within its test.

    [Fact]
    public void LoadGarage_ValidFile_RegistersGarageAndVehicleLocations()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        using var file = new TemporaryGarageFile(CreateGarageJson());

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(id);
        AssertGarageInfo(fixture.Manager, id, "Loaded garage", 3, 1);
        var (vehicles, vehiclesResult) = fixture.Manager.GetVehicles(id);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        var parked = Assert.Single(vehicles);
        Assert.Equal("LOD123", parked.Vehicle.RegistrationNumber);
        Assert.Equal(1, parked.SpotNumber);
        AssertLocation(fixture.Manager, parked.Vehicle, id, "Loaded garage", 1);
        var (garages, listResult) = fixture.Manager.GetGarages();
        Assert.True(listResult.Success, listResult.Message);
        Assert.Equal(fixture.ExpectedGarageCount + 1, garages.Count);
        foreach (var expected in fixture.Garages)
            AssertExistingGarage(fixture.Manager, expected);
    }

    [Fact]
    public void LoadGarage_MissingFile_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"missing-garage-{Guid.NewGuid():N}.json");
        Assert.False(File.Exists(path));

        // Act
        var (id, result) = fixture.Manager.LoadGarage(path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void LoadGarage_MalformedJson_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        using var file = new TemporaryGarageFile("{ this is not valid JSON");

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void LoadGarage_InvalidGarageData_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        using var file = new TemporaryGarageFile(CreateGarageJson(capacity: 0));

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertVehicleNotFound(fixture.Manager, "LOD123");
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void LoadGarage_InvalidVehicleData_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        using var file = new TemporaryGarageFile(CreateGarageJson(registrationNumber: "NOT A REGISTRATION"));

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void LoadGarage_DuplicateGarageName_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        string name = fixture.GetGarage(ManagerGarageKind.Car).ExpectedName;
        using var file = new TemporaryGarageFile(CreateGarageJson(name: name));

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageAlreadyExists, result.Error);
        AssertVehicleNotFound(fixture.Manager, "LOD123");
        AssertManagerUnchanged(fixture);
    }

    [Fact]
    public void LoadGarage_RegistrationAlreadyInAnotherGarage_ReturnsFailureWithoutChangingManager()
    {
        // Arrange
        var fixture = GarageManagerFixtures.CreateManagerWithGarages();
        string registration = fixture.GetGarage(ManagerGarageKind.Car).ExpectedVehicles[0].Vehicle.RegistrationNumber;
        using var file = new TemporaryGarageFile(CreateGarageJson(registrationNumber: registration));

        // Act
        var (id, result) = fixture.Manager.LoadGarage(file.Path);

        // Assert
        Assert.Null(id);
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        AssertManagerUnchanged(fixture);
    }

    // Assertion helpers use only public manager operations and fixture expectations.

    private static IVehicle NewVehicle()
    {
        return VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "NEW123");
    }

    private static void AssertGarageInfo(IGarageManager manager, string id, string name, int capacity, int occupied)
    {
        var (info, result) = manager.GetGarageInfo(id);
        Assert.True(result.Success, result.Message);
        Assert.NotNull(info);
        Assert.Equal(name, info.GarageName);
        Assert.Equal(capacity, info.Capacity);
        Assert.Equal(occupied, info.OccupiedSpaces);
        Assert.Equal(capacity - occupied, info.AvailableSpaces);
    }

    private static void AssertLocation(IGarageManager manager, IVehicle vehicle, string garageId, string garageName, int spotNumber)
    {
        var (location, result) = manager.FindVehicle(vehicle.RegistrationNumber);
        Assert.True(result.Success, result.Message);
        Assert.NotNull(location);
        Assert.Equal(garageId, location.GarageId);
        Assert.Equal(garageName, location.GarageName);
        Assert.Equal(spotNumber, location.SpotNumber);
        Assert.Same(vehicle, location.Vehicle);
    }

    private static void AssertVehicleNotFound(IGarageManager manager, string registrationNumber)
    {
        var (location, result) = manager.FindVehicle(registrationNumber);
        Assert.Null(location);
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
    }

    private static void AssertParkedVehicles(IReadOnlyList<ParkedVehicleInfo> actual, IEnumerable<(int SpotNumber, IVehicle Vehicle)> expected)
    {
        var entries = expected.ToList();
        Assert.Equal(entries.Count, actual.Count);
        foreach (var entry in entries)
        {
            var parked = Assert.Single(actual, candidate => candidate.SpotNumber == entry.SpotNumber);
            Assert.Same(entry.Vehicle, parked.Vehicle);
        }
    }

    private static void AssertLocations(IReadOnlyList<Garage.Manager.Queries.VehicleLocation> actual, IEnumerable<ManagerVehicleFixtureInfo> expected)
    {
        var entries = expected.ToList();
        Assert.Equal(entries.Count, actual.Count);
        foreach (var entry in entries)
        {
            var location = Assert.Single(actual, candidate => ReferenceEquals(candidate.Vehicle, entry.Vehicle));
            Assert.Equal(entry.GarageId, location.GarageId);
            Assert.Equal(entry.GarageName, location.GarageName);
            Assert.Equal(entry.SpotNumber, location.SpotNumber);
        }
    }

    private static void AssertExistingGarage(IGarageManager manager, ManagerGarageFixtureInfo expected, IVehicle? removed = null)
    {
        var remaining = expected.ExpectedVehicles.Where(entry => !ReferenceEquals(entry.Vehicle, removed)).ToList();
        var (vehicles, result) = manager.GetVehicles(expected.GarageId);
        Assert.True(result.Success, result.Message);
        AssertParkedVehicles(vehicles, remaining);
        AssertGarageInfo(manager, expected.GarageId, expected.ExpectedName, expected.ExpectedCapacity, remaining.Count);
        foreach (var entry in remaining)
            AssertLocation(manager, entry.Vehicle, expected.GarageId, expected.ExpectedName, entry.SpotNumber);
    }

    private static void AssertManagerUnchanged(GarageManagerFixtureInfo fixture, IVehicle? removed = null)
    {
        var (garages, result) = fixture.Manager.GetGarages();
        Assert.True(result.Success, result.Message);
        Assert.Equal(fixture.ExpectedGarageCount, garages.Count);
        foreach (var expected in fixture.Garages)
        {
            Assert.Single(garages, garage => garage.Id == expected.GarageId);
            AssertExistingGarage(fixture.Manager, expected, removed);
        }
    }

    private static string CreateGarageJson(string name = "Loaded garage", int capacity = 3, string registrationNumber = "LOD123")
    {
        return JsonSerializer.Serialize(new
        {
            Version = 1,
            Name = name,
            Capacity = capacity,
            Vehicles = new[] { new { VehicleType = "Car", RegistrationNumber = registrationNumber, CountryCode = "SWE" } }
        });
    }

    // Deliberately permits invalid data to exercise the manager's defensive check.
    private sealed class InvalidRegistrationVehicle : IVehicle
    {
        public string RegistrationNumber { get; init; } = string.Empty;
        public CountryCode CountryCode { get; set; } = CountryCode.SWE;
    }

    private sealed class TemporaryGarageFile : IDisposable
    {
        public string Path { get; }

        public TemporaryGarageFile(string json)
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"garage-manager-test-{Guid.NewGuid():N}.json");
            File.WriteAllText(Path, json);
        }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
