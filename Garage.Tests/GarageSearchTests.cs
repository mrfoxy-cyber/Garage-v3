using Garage.Queries;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageSearchTests
{
    [Fact]
    public void Search_CombinesCriteriaWithAndLogic()
    {
        Vehicle matchingCar = VehicleFixtures.CreateCar(
            "MAT001",
            VehicleColor.Black,
            manufacturer: "Volvo");

        Vehicle wrongColor = VehicleFixtures.CreateCar(
            "COL001",
            VehicleColor.Red,
            manufacturer: "Volvo");

        Vehicle wrongType = VehicleFixtures.CreateMotorcycle(
            "TYP001",
            VehicleColor.Black);

        VehicleGarage garage = new(
            "Test garage",
            5,
            [matchingCar, wrongColor, wrongType]);

        VehicleSearchCriteria criteria = new()
        {
            VehicleTypes = [typeof(Car)],
            Manufacturer = "vol",
            Color = VehicleColor.Black,
            NumberOfWheels = 4
        };

        IReadOnlyList<Vehicle> matches = garage.Search(criteria);

        Assert.Same(matchingCar, Assert.Single(matches));
    }

    [Fact]
    public void Search_ReturnsEmptyCollectionWhenNothingMatches()
    {
        VehicleGarage garage = new(
            "Test garage",
            2,
            [VehicleFixtures.CreateCar()]);

        IReadOnlyList<Vehicle> matches = garage.Search(
            new VehicleSearchCriteria
            {
                Color = VehicleColor.Pink
            });

        Assert.Empty(matches);
    }

    [Fact]
    public void VehiclesByType_GroupsAndCountsVehicles()
    {
        VehicleGarage garage = new(
            "Test garage",
            4,
            [
                VehicleFixtures.CreateCar("CAR001"),
                VehicleFixtures.CreateCar("CAR002"),
                VehicleFixtures.CreateMotorcycle("MCY001")
            ]);

        var carGroup = garage.VehiclesByType.Single(
            group => group.VehicleType == nameof(Car));

        var motorcycleGroup = garage.VehiclesByType.Single(
            group => group.VehicleType == nameof(Motorcycle));

        Assert.Equal(2, carGroup.Vehicles.Count());
        Assert.Single(motorcycleGroup.Vehicles);
    }

    [Fact]
    public void Search_FiltersOnVehicleSpecificProperties()
    {
        Vehicle car = VehicleFixtures.CreateCar();
        Vehicle motorcycle = VehicleFixtures.CreateMotorcycle();
        Vehicle airplane = VehicleFixtures.CreateAirplane();
        Vehicle bus = VehicleFixtures.CreateBus();
        Vehicle boat = VehicleFixtures.CreateBoat();
        Vehicle truck = VehicleFixtures.CreateTruck();
        VehicleGarage garage = new(
            "Test garage", 6, [car, motorcycle, airplane, bus, boat, truck]);

        (VehicleSearchCriteria Criteria, Vehicle Expected)[] cases =
        [
            (new() { BodyType = BodyType.Sedan }, car),
            (new() { MotorcycleType = MotorcycleType.Touring }, motorcycle),
            (new() { HasSidecar = false }, motorcycle),
            (new() { BoatType = BoatType.Motorboat }, boat),
            (new() { DraftMeters = 0.9 }, boat),
            (new() { WingspanMeters = 11 }, airplane),
            (new() { MaximumAltitudeMeters = 4_200 }, airplane),
            (new() { StandingPassengerCapacity = 20 }, bus),
            (new() { NumberOfAxles = 3 }, truck),
            (new() { MaximumLoadKg = 25_000 }, truck)
        ];

        foreach (var (criteria, expected) in cases)
            Assert.Same(expected, Assert.Single(garage.Search(criteria)));

        Assert.Empty(garage.Search(new VehicleSearchCriteria { HasSidecar = true }));
        Assert.Empty(garage.Search(new VehicleSearchCriteria { WingspanMeters = 12 }));
    }

    [Fact]
    public void Search_CombinesAircraftAndGeneralFilters()
    {
        Vehicle airplane = VehicleFixtures.CreateAirplane();
        VehicleGarage garage = new("Test garage", 1, [airplane]);

        IReadOnlyList<Vehicle> matches = garage.Search(new VehicleSearchCriteria
        {
            VehicleTypes = [typeof(Airplane)],
            Color = VehicleColor.White,
            WingspanMeters = 11,
            MaximumAltitudeMeters = 4_200
        });

        Assert.Same(airplane, Assert.Single(matches));
    }
}
