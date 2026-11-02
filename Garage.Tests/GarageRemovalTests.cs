using Garage.Tests.Fixtures;
using Garage_V3.Interfaces;
using Garage_V3.results;
using Garage_V3.Services;
using Xunit;
using VehicleGarage = Garage_V3.Garage<Vehicles.Interfaces.IVehicle>;

namespace Garage.Tests;

public class GarageRemovalTests
{
    [Fact]
    public void FindVehicle_RegistrationHasDifferentCase_ReturnsParkedVehicle()
    {
        // Arrange
        var car = VehicleFixtures.CreateCar(
            FixtureKind.Minimal,
            registrationNumber: "ABC123");

        IHandler handler = new GarageHandler();

        var creation = handler.CreateGarage("Test garage", 2);
        Assert.True(creation.Success, creation.Message);

        var parking = handler.ParkVehicle(car);
        Assert.True(parking.Success, parking.Message);

        // Act
        var (match, result) = handler.FindVehicle("AbC123");

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.NotNull(match);
        Assert.Same(car, match);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("abc123")]
    [InlineData("AbC123")]
    public void RemoveVehicle_ParkedRegistration_RemovesVehicleAndFreesSpace(string registration)
    {
        // Arrange
        var car = VehicleFixtures.CreateCar(
            FixtureKind.Minimal,
            registrationNumber: "ABC123");

        VehicleGarage garage = new("Test garage", 2);

        var parking = garage.ParkVehicle(car);
        Assert.True(parking.Success, parking.Message);

        // Act
        var result = garage.RemoveVehicle(registration);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Null(result.Error);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(0, garage.OccupiedSpaces);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Fact]
    public void RemoveVehicle_UnknownRegistration_ReturnsFailureAndKeepsParkedVehicle()
    {
        // Arrange
        var car = VehicleFixtures.CreateCar(FixtureKind.Minimal);
        VehicleGarage garage = new("Test garage", 2);

        var parking = garage.ParkVehicle(car);
        Assert.True(parking.Success, parking.Message);

        // Act
        var result = garage.RemoveVehicle("UNKNOWN");

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleNotFound, result.Error);
        Assert.Same(car, Assert.Single(garage.Vehicles).Vehicle);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(1, garage.AvailableSpaces);
    }
}