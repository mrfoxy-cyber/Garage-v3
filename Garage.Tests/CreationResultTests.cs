using Garage.Services;
using Garage.Results;
using Garage.Tests.Fixtures;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class CreationResultTests
{
    [Fact]
    public void CreateGarage_ReturnsRegisteredGarage()
    {
        GarageService system = new([], []);
        GarageCreationResult result = system.CreateGarage("Test", 2);

        Assert.True(result.Success);
        Assert.Same(result.Garage, Assert.Single(system.Garages));
    }

    [Fact]
    public void CreateGarage_DuplicateAndInvalidCapacityReturnFailure()
    {
        GarageService system = new([new VehicleGarage("Test", 2)], []);

        Assert.False(system.CreateGarage("TEST", 2).Success);
        GarageCreationResult invalid = system.CreateGarage("Other", 0);
        Assert.False(invalid.Success);
        Assert.Null(invalid.Garage);
        Assert.Single(system.Garages);
    }

    [Fact]
    public void CreateVehicle_ReturnsCreatedVehicleOrValidationFailure()
    {
        GarageService service = new([], []);
        var vehicle = VehicleFixtures.CreateCar();
        VehicleCreationResult success = service.VehicleCreation.Create(() => vehicle);
        VehicleCreationResult failure = service.VehicleCreation.Create(() => throw new ArgumentException("Invalid vehicle"));

        Assert.True(success.Success);
        Assert.Same(vehicle, success.Vehicle);
        Assert.False(failure.Success);
        Assert.Null(failure.Vehicle);
        Assert.Equal("Invalid vehicle", failure.Message);
    }

    [Fact]
    public void Park_ReturnsParkingResultWithDisplaySpotNumber()
    {
        VehicleGarage garage = new("Test", 1);
        ParkingResult result = garage.Park(VehicleFixtures.CreateCar());

        Assert.True(result.Success);
        Assert.Equal(1, result.SpotNumber);
        ParkingResult full = garage.Park(VehicleFixtures.CreateCar("RED456"));
        Assert.False(full.Success);
        Assert.Null(full.SpotNumber);
    }
}
