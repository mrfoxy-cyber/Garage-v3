using Garage.Tests.Fixtures;

namespace Garage.Tests;

public class VehicleValidationTests
{
    [Fact]
    public void Bus_AllowsZeroDriverSeatsForAutonomousBus()
    {
        var bus = VehicleFixtures.CreateBus(driverSeats: 0);

        Assert.Equal(0, bus.DriverSeats);
    }

    [Fact]
    public void Bus_RejectsZeroTotalPassengerCapacity()
    {
        Assert.Throws(typeof(ArgumentException), () =>
            VehicleFixtures.CreateBus(
                passengerSeats: 0,
                standingCapacity: 0));
    }

    [Fact]
    public void Airplane_RejectsDualControlsWithOnePilotSeat()
    {
        Assert.Throws(typeof(ArgumentException), () =>
            VehicleFixtures.CreateAirplane(
                pilotSeats: 1,
                hasDualControls: true));
    }

    [Fact]
    public void Boat_RejectsMoreCabinSeatsThanPassengerCapacity()
    {
        Assert.Throws(typeof(ArgumentException), () =>
            VehicleFixtures.CreateBoat(
                passengerCapacity: 4,
                cabinPassengerSeats: 5));
    }

    [Fact]
    public void Truck_RejectsFewerThanTwoWheelsPerAxle()
    {
        Assert.Throws(typeof(ArgumentException), () =>
            VehicleFixtures.CreateTruck(
                numberOfWheels: 4,
                numberOfAxles: 3));
    }
}
