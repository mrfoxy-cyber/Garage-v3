using Garage.Tests.Fixtures;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageConstructionTests
{
    [Fact]
    public void Constructor_SetsCapacity()
    {
        VehicleGarage garage = new("Test garage", 5);

        Assert.Equal(5, garage.Capacity);
        Assert.Equal(5, garage.AvailableSpaces);
        Assert.Equal(garage.Vehicles,[]);
    }

    [Fact]
    public void Constructor_ParksInitialVehicles()
    {
        Vehicle car = VehicleFixtures.CreateCar();
        Vehicle motorcycle = VehicleFixtures.CreateMotorcycle();

        VehicleGarage garage = new(
            "Test garage",
            capacity: 5,
            initialVehicles: [car, motorcycle]);

        Assert.Equal(2, garage.OccupiedSpaces);
        Assert.Contains(car, garage.Vehicles);
        Assert.Contains(motorcycle, garage.Vehicles);
    }

    [Fact]
    public void Constructor_RejectsDuplicateRegistrationsIgnoringCase()
    {
        Vehicle first = VehicleFixtures.CreateCar("ABC123");
        Vehicle second = VehicleFixtures.CreateCar("abc123");

        Assert.Throws(typeof(ArgumentException), () =>
            new VehicleGarage(
                "Test garage",
                capacity: 5,
                initialVehicles: [first, second]));
    }

    [Fact]
    public void Constructor_RejectsNullVehicleInInitialArray()
    {
        List<Vehicle> initialVehicles = [null!];

        Exception exception = Assert.Throws(
            typeof(ArgumentException),
            () => new VehicleGarage(
                "Test garage",
                capacity: 5,
                initialVehicles: initialVehicles));

        Assert.Contains("Initial vehicles must not contain null vehicles", exception.Message);
    }
}
