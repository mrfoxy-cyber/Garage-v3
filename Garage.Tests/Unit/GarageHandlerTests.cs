using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Garage_V3;
using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using Garage_V3.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Vehicles.Interfaces;

namespace Garage.Tests.Unit;

public class GarageHandlerTests
{
    // CreateGarage

    [Fact]
    public void CreateGarage_ValidArguments_ReturnsSuccess()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var result = handler.CreateGarage("Test Garage", 3);

        // Assert
        Assert.True(result.Success, result.Message);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal("Test Garage", info.GarageName);
        Assert.Equal(3, info.Capacity);
        Assert.Equal(0, info.OccupiedSpaces);
        Assert.Equal(3, info.AvailableSpaces);
    }

    [Fact]
    public void CreateGarage_ExistingGarage_ReturnsFailureAndKeepsOriginalGarage()
    {
        // Arrange
        IHandler handler = new GarageHandler();
        var setupResult = handler.CreateGarage("Original Garage", 3);
        Assert.True(setupResult.Success, setupResult.Message);

        // Act
        var result = handler.CreateGarage("Replacement Garage", 10);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageAlreadyExists, result.Error);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal("Original Garage", info.GarageName);
        Assert.Equal(3, info.Capacity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateGarage_InvalidName_ReturnsValidationFailure(string? name)
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var result = handler.CreateGarage(name!, 3);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.Null(info);
        Assert.False(infoResult.Success);
        Assert.Equal(OperationError.NoGarage, infoResult.Error);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(100_001)]
    public void CreateGarage_InvalidCapacity_ReturnsValidationFailure(int capacity)
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var result = handler.CreateGarage("Test Garage", capacity);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.Null(info);
        Assert.False(infoResult.Success);
        Assert.Equal(OperationError.NoGarage, infoResult.Error);
    }

    [Fact]
    public void CreateGarage_AfterFailedCreation_AllowsValidCreation()
    {
        // Arrange
        IHandler handler = new GarageHandler();
        var failedResult = handler.CreateGarage("Invalid Garage", 0);
        Assert.False(failedResult.Success);

        // Act
        var result = handler.CreateGarage("Valid Garage", 5);

        // Assert
        Assert.True(result.Success, result.Message);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal("Valid Garage", info.GarageName);
        Assert.Equal(5, info.Capacity);
        Assert.Equal(0, info.OccupiedSpaces);
    }

    // ParkVehicle

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParkVehicle_NoGarage_ReturnsNoGarageFailure(bool fullVehicle)
    {
        // Arrange
        IHandler handler = new GarageHandler();
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        IVehicle vehicle = VehicleFixtures.CreateCar(kind);

        // Act
        var result = handler.ParkVehicle(vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParkVehicle_ValidVehicle_ParksVehicleAndReturnsSuccess(bool fullVehicle)
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        IVehicle vehicle = VehicleFixtures.CreateMotorcycle(kind);

        // Act
        var result = handler.ParkVehicle(vehicle);

        // Assert
        Assert.True(result.Success, result.Message);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);

        var parked = Assert.Single(vehicles);
        Assert.Same(vehicle, parked.Vehicle);
        Assert.Equal(1, parked.SpotNumber);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParkVehicle_RequestedSpot_ParksAtRequestedSpot(bool fullVehicle)
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler(capacity: 5);
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        IVehicle vehicle = VehicleFixtures.CreateBus(kind);

        // Act
        var result = handler.ParkVehicle(vehicle, 4);

        // Assert
        Assert.True(result.Success, result.Message);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);

        var parked = Assert.Single(vehicles);
        Assert.Same(vehicle, parked.Vehicle);
        Assert.Equal(4, parked.SpotNumber);
    }

    [Fact]
    public void ParkVehicle_InvalidVehicle_ReturnsGarageValidationFailure()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();

        // Act
        var result = handler.ParkVehicle(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Empty(vehicles);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParkVehicle_DuplicateRegistration_ReturnsDuplicateFailure(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 10, kind: kind);
        IHandler handler = fixture.Handler;
        string registration = fixture.ExpectedVehicles[0].Vehicle.RegistrationNumber;
        IVehicle duplicate = VehicleFixtures.CreateCar(kind, registrationNumber: registration);

        // Act
        var result = handler.ParkVehicle(duplicate);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParkVehicle_FullGarage_ReturnsGarageFullFailure(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 6, kind: kind);
        IHandler handler = fixture.Handler;
        IVehicle vehicle = VehicleFixtures.CreateCar(kind, registrationNumber: "NEW123");

        // Act
        var result = handler.ParkVehicle(vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    // FindVehicle

    [Fact]
    public void FindVehicle_NoGarage_ReturnsNullAndNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var (vehicle, result) = handler.FindVehicle("ABC123");

        // Assert
        Assert.Null(vehicle);
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FindVehicle_InvalidRegistration_ReturnsNullAndValidationFailure(string? registrationNumber)
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();

        // Act
        var (vehicle, result) = handler.FindVehicle(registrationNumber!);

        // Assert
        Assert.Null(vehicle);
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FindVehicle_ExistingRegistration_ReturnsSameVehicleAndSuccess(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);

        // Act and Assert — check every vehicle type.
        foreach (var expected in fixture.ExpectedVehicles)
        {
            var (vehicle, result) = fixture.Handler.FindVehicle(expected.Vehicle.RegistrationNumber);

            Assert.True(result.Success, result.Message);
            Assert.Same(expected.Vehicle, vehicle);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FindVehicle_RegistrationWithDifferentCaseAndSpaces_ReturnsMatchingVehicle(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);

        // Act and Assert
        foreach (var expected in fixture.ExpectedVehicles)
        {
            string registration = $"  {expected.Vehicle.RegistrationNumber.ToLowerInvariant()}  ";
            var (vehicle, result) = fixture.Handler.FindVehicle(registration);

            Assert.True(result.Success, result.Message);
            Assert.Same(expected.Vehicle, vehicle);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FindVehicle_UnknownRegistration_ReturnsNullAndVehicleNotFound(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);

        // Act
        var (vehicle, result) = fixture.Handler.FindVehicle("UNKNOWN");

        // Assert
        Assert.Null(vehicle);
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
    }

    // GetGarageInfo

    [Fact]
    public void GetGarageInfo_NoGarage_ReturnsNullAndNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var (info, result) = handler.GetGarageInfo();

        // Assert
        Assert.Null(info);
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Fact]
    public void GetGarageInfo_ExistingGarage_ReturnsCorrectNameAndCapacity()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(name: "My Garage", capacity: 10);

        // Act
        var (info, result) = fixture.Handler.GetGarageInfo();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(info);
        Assert.Equal(fixture.ExpectedName, info.GarageName);
        Assert.Equal(fixture.ExpectedCapacity, info.Capacity);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, info.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, info.AvailableSpaces);
    }

    [Fact]
    public void GetGarageInfo_AfterParkingAndRemoval_ReturnsUpdatedSpaceCounts()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 10);
        IHandler handler = fixture.Handler;
        IVehicle vehicle = VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "NEW123");

        // Act — park.
        var parkingResult = handler.ParkVehicle(vehicle);
        Assert.True(parkingResult.Success, parkingResult.Message);

        // Assert — counts after parking.
        var (afterParking, parkingInfoResult) = handler.GetGarageInfo();
        Assert.True(parkingInfoResult.Success, parkingInfoResult.Message);
        Assert.NotNull(afterParking);
        Assert.Equal(fixture.ExpectedOccupiedSpaces + 1, afterParking.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces - 1, afterParking.AvailableSpaces);

        // Act — remove.
        var removalResult = handler.RemoveVehicle(vehicle.RegistrationNumber);
        Assert.True(removalResult.Success, removalResult.Message);

        // Assert — counts after removal.
        var (afterRemoval, removalInfoResult) = handler.GetGarageInfo();
        Assert.True(removalInfoResult.Success, removalInfoResult.Message);
        Assert.NotNull(afterRemoval);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, afterRemoval.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, afterRemoval.AvailableSpaces);
        Assert.Equal(fixture.ExpectedCapacity, afterRemoval.Capacity);
    }

    // GetVehicles

    [Fact]
    public void GetVehicles_NoGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var (vehicles, result) = handler.GetVehicles();

        // Assert
        Assert.Empty(vehicles);
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Fact]
    public void GetVehicles_EmptyGarage_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();

        // Act
        var (vehicles, result) = handler.GetVehicles();

        // Assert
        Assert.Empty(vehicles);
        Assert.True(result.Success, result.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetVehicles_ParkedVehicles_ReturnsCorrectVehiclesAndSpotNumbers(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);

        // Act
        var (vehicles, result) = fixture.Handler.GetVehicles();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(fixture.ExpectedVehicles.Count, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetVehicles_AfterRemoval_ExcludesRemovedVehicle(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);
        var target = fixture.ExpectedVehicles[0];

        var removalResult = fixture.Handler.RemoveVehicle(target.Vehicle.RegistrationNumber);
        Assert.True(removalResult.Success, removalResult.Message);

        // Act
        var (vehicles, result) = fixture.Handler.GetVehicles();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(fixture.ExpectedVehicles.Count - 1, vehicles.Count);
        Assert.DoesNotContain(vehicles, entry => ReferenceEquals(entry.Vehicle, target.Vehicle));

        foreach (var expected in fixture.ExpectedVehicles.Skip(1))
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    // GetVehicleTypeCounts

    [Fact]
    public void GetVehicleTypeCounts_NoGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var (counts, result) = handler.GetVehicleTypeCounts();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(counts);
    }

    [Fact]
    public void GetVehicleTypeCounts_EmptyGarage_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();

        // Act
        var (counts, result) = handler.GetVehicleTypeCounts();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(counts);
    }

    [Fact]
    public void GetVehicleTypeCounts_MixedVehicles_ReturnsCorrectCountForEachType()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 30, variedCounts: true);
        var expectedCounts = GarageFixtures.VariedTypeCounts;

        // Act
        var (counts, result) = fixture.Handler.GetVehicleTypeCounts();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedCounts.Count, counts.Count);
        Assert.Equal(expectedCounts.Values.Sum(), counts.Sum(item => item.Count));

        foreach (var expected in expectedCounts)
        {
            var actual = Assert.Single(counts, item => item.VehicleType == expected.Key);
            Assert.Equal(expected.Value, actual.Count);
        }
    }

    [Fact]
    public void GetVehicleTypeCounts_MixedVehicles_ReturnsTypesInAlphabeticalOrder()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 30, variedCounts: true);

        // Act
        var (counts, result) = fixture.Handler.GetVehicleTypeCounts();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(GarageFixtures.ExpectedTypeOrder, counts.Select(item => item.VehicleType));
    }

    // RemoveVehicle

    [Fact]
    public void RemoveVehicle_NoGarage_ReturnsNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var result = handler.RemoveVehicle("ABC123");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Fact]
    public void RemoveVehicle_ExistingRegistration_RemovesVehicleAndReturnsSuccess()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles();
        var target = fixture.ExpectedVehicles[0];

        // Act
        var result = fixture.Handler.RemoveVehicle(target.Vehicle.RegistrationNumber);

        // Assert
        Assert.True(result.Success, result.Message);

        var (vehicles, vehiclesResult) = fixture.Handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces - 1, vehicles.Count);
        Assert.DoesNotContain(vehicles, entry => ReferenceEquals(entry.Vehicle, target.Vehicle));

        foreach (var expected in fixture.ExpectedVehicles.Skip(1))
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    [Fact]
    public void RemoveVehicle_UnknownRegistration_ReturnsVehicleNotFound()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles();

        // Act
        var result = fixture.Handler.RemoveVehicle("UNKNOWN");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);

        var (vehicles, vehiclesResult) = fixture.Handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
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
        var fixture = GarageFixtures.CreateHandlerWithVehicles();

        // Act
        var result = fixture.Handler.RemoveVehicle(registrationNumber!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (vehicles, vehiclesResult) = fixture.Handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }
    }

    // SearchVehicles

    [Fact]
    public void SearchVehicles_NoGarage_ReturnsEmptyCollectionAndNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();
        VehicleSearchCriteria criteria = new();

        // Act
        var (vehicles, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_NullCriteria_ReturnsEmptyCollectionAndValidationFailure()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles();

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(vehicles);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SearchVehicles_NoFilters_ReturnsAllParkedVehicles(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: kind);
        VehicleSearchCriteria criteria = new();

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertSearchMatches(vehicles, fixture.ExpectedVehicles);
    }

    [Theory]
    [InlineData("RegistrationNumber")]
    [InlineData("VehicleType")]
    [InlineData("Manufacturer")]
    [InlineData("Model")]
    [InlineData("CountryCode")]
    [InlineData("ManufacturingYear")]
    [InlineData("Color")]
    [InlineData("NumberOfWheels")]
    public void SearchVehicles_SingleFilter_ReturnsOnlyMatchingVehicles(string filter)
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Full);

        (VehicleSearchCriteria Criteria, Type[] Types) testCase = filter switch
        {
            "RegistrationNumber" => (new() { RegistrationNumber = "ABC123" }, [typeof(Car)]),
            "VehicleType" => (new() { VehicleType = "Car" }, [typeof(Car)]),
            "Manufacturer" => (new() { Manufacturer = "Volvo" }, [typeof(Car), typeof(Truck)]),
            "Model" => (new() { Model = "V70" }, [typeof(Car)]),
            "CountryCode" => (new() { CountryCode = CountryCode.SWE },
                [typeof(Car), typeof(Motorcycle), typeof(Bus), typeof(Airplane), typeof(Boat), typeof(Truck)]),
            "ManufacturingYear" => (new() { ManufacturingYear = 2020 }, [typeof(Car)]),
            "Color" => (new() { Color = VehicleColor.White }, [typeof(Airplane), typeof(Boat)]),
            "NumberOfWheels" => (new() { NumberOfWheels = 6 }, [typeof(Bus), typeof(Truck)]),
            _ => throw new ArgumentOutOfRangeException(nameof(filter))
        };

        var expected = fixture.ExpectedVehicles
            .Where(entry => testCase.Types.Contains(entry.Vehicle.GetType()))
            .ToList();

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(testCase.Criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertSearchMatches(vehicles, expected);
    }

    [Fact]
    public void SearchVehicles_CombinedFilters_RequiresAllCriteriaToMatch()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler(capacity: 4);

        IVehicle matching = VehicleFixtures.CreateCar(
            registrationNumber: "MAT001", color: VehicleColor.Black,
            manufacturer: "Volvo", manufacturingYear: 2020);

        IVehicle wrongColor = VehicleFixtures.CreateCar(
            registrationNumber: "COL001", color: VehicleColor.Red,
            manufacturer: "Volvo", manufacturingYear: 2020);

        IVehicle wrongManufacturer = VehicleFixtures.CreateCar(
            registrationNumber: "MAN001", color: VehicleColor.Black,
            manufacturer: "Saab", manufacturingYear: 2020);

        IVehicle wrongYear = VehicleFixtures.CreateCar(
            registrationNumber: "YEA001", color: VehicleColor.Black,
            manufacturer: "Volvo", manufacturingYear: 2021);

        foreach (IVehicle vehicle in new[] { matching, wrongColor, wrongManufacturer, wrongYear })
        {
            var setupResult = handler.ParkVehicle(vehicle);
            Assert.True(setupResult.Success, setupResult.Message);
        }

        VehicleSearchCriteria criteria = new()
        {
            Manufacturer = "Volvo",
            Color = VehicleColor.Black,
            ManufacturingYear = 2020
        };

        // Act
        var (vehicles, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        var match = Assert.Single(vehicles);
        Assert.Same(matching, match.Vehicle);
    }

    [Fact]
    public void SearchVehicles_NoMatches_ReturnsEmptyCollectionAndSuccess()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Full);
        VehicleSearchCriteria criteria = new() { Color = VehicleColor.Pink };

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_TextFilters_IgnoreCaseAndTrimSearchValues()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Full);
        var expected = fixture.ExpectedVehicles.Where(entry => entry.Vehicle is Car).ToList();

        VehicleSearchCriteria criteria = new()
        {
            RegistrationNumber = "  abc123  ",
            VehicleType = "  car  ",
            Manufacturer = "  volvo  ",
            Model = "  v70  "
        };

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertSearchMatches(vehicles, expected);
    }

    [Theory]
    [InlineData("RegistrationNumber", "ABC")]
    [InlineData("VehicleType", "Ca")]
    [InlineData("Manufacturer", "Vol")]
    [InlineData("Model", "V7")]
    public void SearchVehicles_TextFilter_RequiresWholeValueMatch(string filter, string value)
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Full);

        VehicleSearchCriteria criteria = filter switch
        {
            "RegistrationNumber" => new() { RegistrationNumber = value },
            "VehicleType" => new() { VehicleType = value },
            "Manufacturer" => new() { Manufacturer = value },
            "Model" => new() { Model = value },
            _ => throw new ArgumentOutOfRangeException(nameof(filter))
        };

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_BlankTextFilters_AreIgnored()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Full);
        VehicleSearchCriteria criteria = new()
        {
            RegistrationNumber = "",
            VehicleType = "   ",
            Manufacturer = "\t",
            Model = " "
        };

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertSearchMatches(vehicles, fixture.ExpectedVehicles);
    }

    [Theory]
    [InlineData("Manufacturer")]
    [InlineData("Model")]
    [InlineData("ManufacturingYear")]
    [InlineData("Color")]
    [InlineData("NumberOfWheels")]
    public void SearchVehicles_MissingOptionalDetails_DoesNotMatchSpecifiedFilter(string filter)
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(kind: FixtureKind.Minimal);

        VehicleSearchCriteria criteria = filter switch
        {
            "Manufacturer" => new() { Manufacturer = "Volvo" },
            "Model" => new() { Model = "V70" },
            "ManufacturingYear" => new() { ManufacturingYear = 2020 },
            "Color" => new() { Color = VehicleColor.Black },
            "NumberOfWheels" => new() { NumberOfWheels = 4 },
            _ => throw new ArgumentOutOfRangeException(nameof(filter))
        };

        // Act
        var (vehicles, result) = fixture.Handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void SearchVehicles_WheelFilter_ExcludesVehiclesWithoutWheelCapability()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler(capacity: 2);
        IVehicle car = VehicleFixtures.CreateCar(FixtureKind.Full);
        IVehicle boat = VehicleFixtures.CreateBoat(FixtureKind.Full);

        var carResult = handler.ParkVehicle(car);
        Assert.True(carResult.Success, carResult.Message);

        var boatResult = handler.ParkVehicle(boat);
        Assert.True(boatResult.Success, boatResult.Message);

        VehicleSearchCriteria criteria = new() { NumberOfWheels = 4 };

        // Act
        var (vehicles, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        var match = Assert.Single(vehicles);
        Assert.Same(car, match.Vehicle);
    }

    [Fact]
    public void SearchVehicles_MatchingVehicles_PreservesSpotNumbers()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler(capacity: 10);
        IVehicle airplane = VehicleFixtures.CreateAirplane(FixtureKind.Full);
        IVehicle boat = VehicleFixtures.CreateBoat(FixtureKind.Full);
        IVehicle car = VehicleFixtures.CreateCar(FixtureKind.Full);

        var airplaneResult = handler.ParkVehicle(airplane, 8);
        Assert.True(airplaneResult.Success, airplaneResult.Message);

        var boatResult = handler.ParkVehicle(boat, 2);
        Assert.True(boatResult.Success, boatResult.Message);

        var carResult = handler.ParkVehicle(car, 5);
        Assert.True(carResult.Success, carResult.Message);

        VehicleSearchCriteria criteria = new() { Color = VehicleColor.White };

        // Act
        var (vehicles, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        AssertSearchMatches(vehicles, [(8, airplane), (2, boat)]);
    }

    // Shared assertions for returned vehicles and their original spots.
    private static void AssertSearchMatches(IReadOnlyList<ParkedVehicleInfo> actual, IEnumerable<(int SpotNumber, IVehicle Vehicle)> expected)
    {
        var expectedVehicles = expected.ToList();
        Assert.Equal(expectedVehicles.Count, actual.Count);

        foreach (var entry in expectedVehicles)
        {
            var match = Assert.Single(actual, parked => ReferenceEquals(parked.Vehicle, entry.Vehicle));
            Assert.Equal(entry.SpotNumber, match.SpotNumber);
        }
    }
    // InitializeVehicles

    [Fact]
    public void InitializeVehicles_NoGarage_ReturnsNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();
        var initialVehicles = VehicleFixtures.CreateAll(FixtureKind.Minimal);

        // Act
        var result = handler.InitializeVehicles(initialVehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InitializeVehicles_ValidCollection_PopulatesGarageAndReturnsSuccess(bool fullVehicle)
    {
        // Arrange
        FixtureKind kind = fullVehicle ? FixtureKind.Full : FixtureKind.Minimal;
        var initialVehicles = VehicleFixtures.CreateAll(kind);
        IHandler handler = GarageFixtures.CreateHandler(capacity: initialVehicles.Count + 2);

        // Act
        var result = handler.InitializeVehicles(initialVehicles);

        // Assert
        Assert.True(result.Success, result.Message);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(initialVehicles.Count, vehicles.Count);

        for (int index = 0; index < initialVehicles.Count; index++)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == index + 1);
            Assert.Same(initialVehicles[index], parked.Vehicle);
        }

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(initialVehicles.Count, info.OccupiedSpaces);
        Assert.Equal(2, info.AvailableSpaces);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InitializeVehicles_InvalidCollection_ReturnsValidationFailure(bool containsNullVehicle)
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();
        IEnumerable<IVehicle>? initialVehicles = containsNullVehicle
            ? new IVehicle[] { VehicleFixtures.CreateCar(FixtureKind.Minimal), null! }
            : null;

        // Act
        var result = handler.InitializeVehicles(initialVehicles!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Empty(vehicles);
    }

    [Fact]
    public void InitializeVehicles_DuplicateRegistrations_ReturnsFailureAndLeavesGarageEmpty()
    {
        // Arrange
        IHandler handler = GarageFixtures.CreateHandler();
        IVehicle[] initialVehicles =
        [
            VehicleFixtures.CreateBus(FixtureKind.Minimal),
        VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "ABC123"),
        VehicleFixtures.CreateMotorcycle(FixtureKind.Minimal, registrationNumber: "abc123")
        ];

        // Act
        var result = handler.InitializeVehicles(initialVehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Empty(vehicles);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(0, info.OccupiedSpaces);
        Assert.Equal(info.Capacity, info.AvailableSpaces);
    }

    [Fact]
    public void InitializeVehicles_OverCapacity_ReturnsFailureAndLeavesGarageEmpty()
    {
        // Arrange
        var initialVehicles = VehicleFixtures.CreateAll(FixtureKind.Minimal);
        IHandler handler = GarageFixtures.CreateHandler(capacity: initialVehicles.Count - 1);

        // Act
        var result = handler.InitializeVehicles(initialVehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);

        var (vehicles, vehiclesResult) = handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Empty(vehicles);

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(0, info.OccupiedSpaces);
        Assert.Equal(info.Capacity, info.AvailableSpaces);
    }

    [Fact]
    public void InitializeVehicles_NonEmptyGarage_ReturnsFailureAndKeepsExistingVehicles()
    {
        // Arrange
        var fixture = GarageFixtures.CreateHandlerWithVehicles(capacity: 10);
        IVehicle[] initialVehicles =
        [
            VehicleFixtures.CreateCar(FixtureKind.Minimal, registrationNumber: "NEW123")
        ];

        // Act
        var result = fixture.Handler.InitializeVehicles(initialVehicles);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);

        var (vehicles, vehiclesResult) = fixture.Handler.GetVehicles();
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, vehicles.Count);

        foreach (var expected in fixture.ExpectedVehicles)
        {
            var parked = Assert.Single(vehicles, entry => entry.SpotNumber == expected.SpotNumber);
            Assert.Same(expected.Vehicle, parked.Vehicle);
        }

        var (info, infoResult) = fixture.Handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(fixture.ExpectedOccupiedSpaces, info.OccupiedSpaces);
        Assert.Equal(fixture.ExpectedAvailableSpaces, info.AvailableSpaces);
    }
}
