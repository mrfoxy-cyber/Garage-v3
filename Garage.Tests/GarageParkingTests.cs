using Garage.Factories;
using Garage.Vehicles;
using Garage_V3.results;
using Vehicles.Interfaces;
using Xunit;
using VehicleGarage = Garage_V3.Garage<Vehicles.Interfaces.IVehicle>;

namespace Garage.Tests;

public sealed class GarageParkingTests
{
    [Fact]
    public void ParkVehicle_AvailableSpace_AddsVehicleAndUpdatesCounts()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 2);
        IVehicle vehicle = CreateVehicle();

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Null(result.Error);
        Assert.Equal(1, result.SpotNumber);
        var entry = Assert.Single(garage.Vehicles);
        Assert.Same(vehicle, entry.Vehicle);
        Assert.Equal(1, entry.SpotNumber);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(1, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_GarageFull_ReturnsFailureWithoutChangingContents()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 1);
        IVehicle first = CreateVehicle("ONE123");
        var firstResult = garage.ParkVehicle(first);
        Assert.True(firstResult.Success, firstResult.Message);

        // Act
        var result = garage.ParkVehicle(CreateVehicle("TWO123"));

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.GarageFull, result.Error);
        Assert.Null(result.SpotNumber);
        Assert.Same(first, Assert.Single(garage.Vehicles).Vehicle);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(0, garage.AvailableSpaces);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("abc123")]
    [InlineData("AbC123")]
    public void ParkVehicle_DuplicateRegistration_ReturnsFailureIgnoringCase(string duplicateRegistration)
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 2);
        IVehicle first = CreateVehicle("ABC123");
        Assert.True(garage.ParkVehicle(first).Success);

        // The test vehicle preserves casing, so this tests Garage rather than Vehicle normalization.
        IVehicle duplicate = CreateVehicle(duplicateRegistration);

        // Act
        var result = garage.ParkVehicle(duplicate);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        Assert.Null(result.SpotNumber);
        Assert.Same(first, Assert.Single(garage.Vehicles).Vehicle);
        Assert.Equal(1, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_RequestedSpotAvailable_UsesRequestedSpot()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 3);
        IVehicle vehicle = CreateVehicle();

        // Act
        var result = garage.ParkVehicle(vehicle, spotNumber: 2);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.SpotNumber);
        var entry = Assert.Single(garage.Vehicles);
        Assert.Equal(2, entry.SpotNumber);
        Assert.Same(vehicle, entry.Vehicle);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_RequestedSpotOccupied_ReturnsFailureAndKeepsOriginalVehicle()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 2);
        IVehicle first = CreateVehicle("ONE123");
        Assert.True(garage.ParkVehicle(first, spotNumber: 1).Success);

        // Act
        var result = garage.ParkVehicle(CreateVehicle("TWO123"), spotNumber: 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.SpotOccupied, result.Error);
        Assert.Null(result.SpotNumber);
        var entry = Assert.Single(garage.Vehicles);
        Assert.Equal(1, entry.SpotNumber);
        Assert.Same(first, entry.Vehicle);
        Assert.Equal(1, garage.AvailableSpaces);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void ParkVehicle_RequestedSpotOutsideCapacity_ReturnsFailureAndDoesNotConsumeSpace(int spotNumber)
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 3);
        IVehicle vehicle = CreateVehicle();

        // Act
        var result = garage.ParkVehicle(vehicle, spotNumber);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.InvalidSpot, result.Error);
        Assert.Null(result.SpotNumber);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(3, garage.AvailableSpaces);

        var retry = garage.ParkVehicle(vehicle, spotNumber: 1);
        Assert.True(retry.Success, retry.Message);
    }

    [Fact]
    public void ParkVehicle_NullVehicle_ReturnsValidationFailureWithoutChangingCounts()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 2);

        // Act
        var result = garage.ParkVehicle(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void ParkVehicle_BlankRegistration_ReturnsValidationFailure(string registration)
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 2);
        IVehicle vehicle = CreateVehicle(registration);

        // Act
        var result = garage.ParkVehicle(vehicle);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Fact]
    public void ParkVehicle_NoRequestedSpot_UsesLowestAvailableSpotBetweenOccupiedSpots()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 4);
        Assert.True(garage.ParkVehicle(CreateVehicle("ONE123"), spotNumber: 3).Success);
        Assert.True(garage.ParkVehicle(CreateVehicle("TWO123"), spotNumber: 1).Success);
        IVehicle third = CreateVehicle("THR123");

        // Act
        var result = garage.ParkVehicle(third);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.SpotNumber);
        Assert.Contains(garage.Vehicles, entry => entry.SpotNumber == 2 && ReferenceEquals(entry.Vehicle, third));
        Assert.Equal(3, garage.OccupiedSpaces);
        Assert.Equal(1, garage.AvailableSpaces);
    }

    [Fact]
    public void RemoveVehicle_ParkedVehicle_ReleasesItsSpotForTheNextVehicle()
    {
        // Arrange
        VehicleGarage garage = new("Test garage", 1);
        IVehicle first = CreateVehicle("ONE123");
        IVehicle second = CreateVehicle("TWO123");
        Assert.True(garage.ParkVehicle(first).Success);

        // Act
        var removal = garage.RemoveVehicle("one123");

        // Assert
        Assert.True(removal.Success, removal.Message);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(1, garage.AvailableSpaces);

        var parking = garage.ParkVehicle(second);
        Assert.True(parking.Success, parking.Message);
        Assert.Equal(1, parking.SpotNumber);
        Assert.Same(second, Assert.Single(garage.Vehicles).Vehicle);
    }

    [Fact]
    public void ParkVehicle_MixedVehicleTypes_AcceptsEachAndEnumeratesVehiclesWithoutSpotTuples()
    {
        // Arrange
        IVehicle[] vehicles =
        [
            CarFactory.CreateCar("ABC123", CountryCode.SWE),
            MotorcycleFactory.CreateMotorcycle("MCY123", CountryCode.SWE),
            BusFactory.CreateBus("BUS123", CountryCode.SWE),
            BoatFactory.CreateBoat("SABC", CountryCode.SWE),
            AirplaneFactory.CreateAirplane("SE-ABC", CountryCode.SWE),
            TruckFactory.CreateTruck("TRK123", CountryCode.SWE)
        ];

        VehicleGarage garage = new("Mixed garage", vehicles.Length);

        // Act
        foreach (IVehicle vehicle in vehicles)
        {
            var result = garage.ParkVehicle(vehicle);
            Assert.True(result.Success, result.Message);
        }

        IVehicle[] enumerated = garage.ToArray();

        // Assert
        Assert.Equal(vehicles.Length, enumerated.Length);
        Assert.Equal(vehicles.Length, garage.OccupiedSpaces);
        Assert.Equal(0, garage.AvailableSpaces);

        for (int index = 0; index < vehicles.Length; index++)
        {
            Assert.Same(vehicles[index], enumerated[index]);
            Assert.Equal(index + 1, garage.Vehicles[index].SpotNumber);
        }
    }

    private static IVehicle CreateVehicle(string registration = "ABC123") => new TestVehicle { RegistrationNumber = registration };

    // Keeps garage unit tests independent of rich factory data and registration normalization.
    private sealed class TestVehicle : IVehicle
    {
        public string RegistrationNumber { get; init; } = string.Empty;
        public CountryCode CountryCode { get; set; } = CountryCode.SWE;
    }
}
