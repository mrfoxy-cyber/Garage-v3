using Garage.Factories;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage_V3;
using Vehicles.Interfaces;
using Garage_V3.results;

namespace Garage.Tests;

public class GarageConstructionTests
{
    [Fact]
    public void Constructor_SetsCapacity()
    {
        Garage<IVehicle> garage = new("Test garage", 5);

        Assert.Equal(5, garage.Capacity);
        Assert.Equal(5, garage.AvailableSpaces);
        Assert.Equal(garage.Vehicles,[]);
    }

    [Fact]
    public void Constructor_ParksInitialVehicles()
    {
        IVehicle car = VehicleFixtures.CreateCar();
        IVehicle motorcycle = VehicleFixtures.CreateMotorcycle();

        Garage<IVehicle> garage = new Garage<IVehicle>("Test garage", 5);

        garage.InitializeVehicles([car, motorcycle]);

        Assert.Equal(2, garage.OccupiedSpaces);

        // Verify that the vehicles are parked in the order they were added
        //Observe that the garage.Vehicles is a list of tuples (spaceNumber, vehicle)
        Assert.Contains((1, car), garage.Vehicles);
        Assert.Contains((2, motorcycle), garage.Vehicles);
        Assert.Equal(car, garage.Vehicles[0].Vehicle);
        Assert.Equal(motorcycle, garage.Vehicles[1].Vehicle);
        Assert.Equal(2, garage.OccupiedSpaces);
    }

    [Fact]
    public void Constructor_RejectsDuplicateRegistrationsIgnoringCase()
    {
        IVehicle first = CarFactory.CreateCar("ABC123", CountryCode.SWE);
        IVehicle second = CarFactory.CreateCar("abc123", CountryCode.SWE);

        Garage<IVehicle> garage = new("Test garage", 5);
        ParkingResult firstResult = garage.ParkVehicle(first);
        ParkingResult secondResult = garage.ParkVehicle(second);

        Assert.False(secondResult.Success);
        Assert.Equal(OperationError.DuplicateRegistration, secondResult.Error);

    }
}
