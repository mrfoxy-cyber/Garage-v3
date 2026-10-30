using Garage.Tests.Fixtures;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageRemovalTests
{
    [Fact]
    public void FindVehicleByRegistration_IgnoresCase()
    {
        Vehicle car = VehicleFixtures.CreateCar("ABC123");
        VehicleGarage garage = new("Test garage", 2, [car]);

        var match = garage.FindVehicleByRegistration("AbC123");

        Assert.NotNull(match);
        Assert.Same(car, match.Vehicle);
    }

    [Fact]
    public void Remove_RemovesVehicleAndFreesSpace()
    {
        Vehicle car = VehicleFixtures.CreateCar();
        VehicleGarage garage = new("Test garage", 2, [car]);

        var result = garage.Remove(car.RegistrationNumber);

        Assert.True(result.Success);
        Assert.Empty(garage.Vehicles);
        Assert.Equal(2, garage.AvailableSpaces);
    }

    [Fact]
    public void Remove_ReturnsFailureForUnknownRegistration()
    {
        VehicleGarage garage = new("Test garage", 2);

        var result = garage.Remove("UNKNOWN");

        Assert.False(result.Success);
        Assert.Empty(garage.Vehicles);
    }
}
