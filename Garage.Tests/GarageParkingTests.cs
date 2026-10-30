using Garage.Models;
using Garage.Services;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageParkingTests
{
    [Fact]
    public void Park_AddsVehicleAndUpdatesSpaceCounts()
    {
        VehicleGarage garage = new("Test garage", 2);
        Vehicle car = VehicleFixtures.CreateCar();

        var result = garage.Park(car);

        Assert.True(result.Success);
        Assert.Contains(car, garage.Vehicles);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(1, garage.AvailableSpaces);
    }

    [Fact]
    public void Park_FailsWhenGarageIsFull()
    {
        VehicleGarage garage = new("Test garage", 1);
        garage.Park(VehicleFixtures.CreateCar("ONE123"));

        var result = garage.Park(
            VehicleFixtures.CreateCar("TWO123"));

        Assert.False(result.Success);
        Assert.Single(garage.Vehicles);
    }

    [Fact]
    public void Park_RejectsDuplicateRegistrationIgnoringCase()
    {
        VehicleGarage garage = new("Test garage", 2);
        garage.Park(VehicleFixtures.CreateCar("ABC123"));

        var result = garage.Park(
            VehicleFixtures.CreateCar("abc123"));

        Assert.False(result.Success);
        Assert.Single(garage.Vehicles);
    }

    [Fact]
    public void ParkAtSpot_UsesRequestedSpot()
    {
        VehicleGarage garage = new("Test garage", 3);
        Vehicle car = VehicleFixtures.CreateCar();

        var result = garage.Park(car, spot: 2);
        var parkedVehicle = Assert.Single(garage.ParkedVehicles);

        Assert.True(result.Success);
        Assert.Equal(2, parkedVehicle.SpotNumber);
        Assert.Same(car, parkedVehicle.Vehicle);
    }

    [Fact]
    public void ParkAtSpot_FailsWhenSpotIsOccupied()
    {
        VehicleGarage garage = new("Test garage", 2);
        garage.Park(VehicleFixtures.CreateCar("ONE123"), spot: 1);

        var result = garage.Park(
            VehicleFixtures.CreateCar("TWO123"),
            spot: 1);

        Assert.False(result.Success);
        Assert.Single(garage.Vehicles);
    }

    [Fact]
    public void Park_RejectsNullVehicle()
    {
        VehicleGarage garage = new("Test garage", 2);

        Exception exception = Assert.Throws(
            typeof(ArgumentNullException),
            () => garage.Park(null!));

        Assert.Contains("vehicle", exception.Message);
        Assert.Empty(garage.Vehicles);
    }

    [Fact]
    public void ParkAtSpot_RejectsNullVehicle()
    {
        VehicleGarage garage = new("Test garage", 2);

        Exception exception = Assert.Throws(
            typeof(ArgumentNullException),
            () => garage.Park(null!, spot: 1));

        Assert.Contains("vehicle", exception.Message);
        Assert.Empty(garage.Vehicles);
    }

    [Fact]
    public void Park_SkipsOpenSpotThatIsTooSmall()
    {
        VehicleGarage garage = new("Sized garage",
        [
            new Dimensions(4, 2, 2),
            new Dimensions(5, 2, 2)
        ]);
        Vehicle car = VehicleFixtures.CreateCar();

        Assert.Equal([2], garage.AvailableSpotsFor(car));

        var result = garage.Park(car);

        Assert.True(result.Success);
        Assert.Equal(2, result.SpotNumber);
        Assert.Equal(2, Assert.Single(garage.ParkedVehicles).SpotNumber);
        Assert.Empty(garage.AvailableSpotsFor(car));
    }

    [Fact]
    public void ParkAtSpot_RejectsVehicleThatDoesNotFitAndKeepsSpotFree()
    {
        VehicleGarage garage = new("Sized garage",
        [
            new Dimensions(2, 5, 2),
            new Dimensions(4.8, 1.8, 1.5)
        ]);
        Vehicle car = VehicleFixtures.CreateCar();

        var tooSmall = garage.Park(car, spot: 1);
        var exactFit = garage.Park(car, spot: 2);

        Assert.False(tooSmall.Success);
        Assert.Contains("does not fit", tooSmall.Message);
        Assert.True(exactFit.Success);
        Assert.Equal(1, garage.OccupiedSpaces);
        Assert.Equal(2, Assert.Single(garage.ParkedVehicles).SpotNumber);
    }

    [Fact]
    public void Remove_MakesSizedSpotAvailableAgain()
    {
        VehicleGarage garage = new("Sized garage", [new Dimensions(5, 2, 2)]);
        Vehicle first = VehicleFixtures.CreateCar("ONE123");
        Vehicle second = VehicleFixtures.CreateCar("TWO123");

        Assert.True(garage.Park(first).Success);
        Assert.True(garage.Remove(first.RegistrationNumber).Success);

        Assert.Equal([1], garage.AvailableSpotsFor(second));
        Assert.True(garage.Park(second).Success);
        Assert.Equal(new Dimensions(5, 2, 2), Assert.Single(garage.SpotDetails).Dimensions);
    }

    [Theory]
    [InlineData(0, 2, 2)]
    [InlineData(-1, 2, 2)]
    [InlineData(double.NaN, 2, 2)]
    [InlineData(2, double.PositiveInfinity, 2)]
    [InlineData(2, 2, double.NegativeInfinity)]
    public void Dimensions_RejectsInvalidMeasurements(
        double lengthMeters,
        double widthMeters,
        double heightMeters)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Dimensions(lengthMeters, widthMeters, heightMeters));
    }

    [Fact]
    public void Constructor_RejectsMissingSpotDimensions()
    {
        IReadOnlyList<Dimensions> sizes = [null!];

        Assert.Throws<ArgumentException>(() =>
            new VehicleGarage("Sized garage", sizes));
    }

    [Fact]
    public void ParkingMenu_OnlyOffersVehiclesThatFitTheChosenSpot()
    {
        VehicleGarage garage = new("Sized garage",
        [
            new Dimensions(5, 2, 2),
            new Dimensions(15, 4, 4)
        ]);
        Vehicle car = VehicleFixtures.CreateCar();
        Vehicle boat = VehicleFixtures.CreateBoat();
        GarageService service = new([garage], [car, boat]);

        Assert.Equal([car], service.GetParkableVehicles(garage, spotNumber: 1));
        Assert.Equal([car, boat], service.GetParkableVehicles(garage));

        garage.Park(car, spot: 1);

        Assert.Equal([boat], service.GetParkableVehicles(garage));
    }
}
