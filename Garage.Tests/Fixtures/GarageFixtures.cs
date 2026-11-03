using Garage_V3;
using Garage_V3.Interfaces;
using Garage_V3.Services;
using Vehicles.Interfaces;

namespace Garage.Tests.Fixtures;

internal sealed record GarageFixtureInfo(
    Garage<IVehicle> Garage,
    string ExpectedName,
    int ExpectedCapacity,
    IReadOnlyList<(int SpotNumber, IVehicle Vehicle)> ExpectedVehicles)
{
    public int ExpectedOccupiedSpaces => ExpectedVehicles.Count;
    public int ExpectedAvailableSpaces => ExpectedCapacity - ExpectedOccupiedSpaces;
}

internal static class GarageFixtures
{

    public static Garage<IVehicle> CreateGarage(string name = "Test garage", int capacity = 6)
    {
        return new Garage<IVehicle>(name, capacity);
    }

    public static IHandler CreateHandler(string name = "Test garage", int capacity = 6)
    {
        IHandler handler = new GarageHandler();
        var result = handler.CreateGarage(name, capacity);

        if (!result.Success)
            throw new InvalidOperationException($"Fixture setup failed: {result.Message}");

        return handler;
    }

    public static HandlerFixtureInfo CreateHandlerWithVehicles(string name = "Test garage", int capacity = 6, FixtureKind kind = FixtureKind.Minimal, bool variedCounts = false)
    {
        var vehicles = CreateFixtureVehicles(kind, variedCounts);

        if (capacity < vehicles.Count)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must fit all fixture vehicles.");

        IHandler handler = CreateHandler(name, capacity);
        List<(int SpotNumber, IVehicle Vehicle)> expectedVehicles = [];

        for (int index = 0; index < vehicles.Count; index++)
        {
            IVehicle vehicle = vehicles[index];
            int spotNumber = index + 1;

            var result = handler.ParkVehicle(vehicle, spotNumber);

            if (!result.Success)
                throw new InvalidOperationException($"Fixture setup failed: {result.Message}");

            expectedVehicles.Add((spotNumber, vehicle));
        }

        return new HandlerFixtureInfo(handler, name.Trim(), capacity, expectedVehicles.AsReadOnly());
    }


    internal sealed record HandlerFixtureInfo(
    IHandler Handler,
    string ExpectedName,
    int ExpectedCapacity,
    IReadOnlyList<(int SpotNumber, IVehicle Vehicle)> ExpectedVehicles)
    {
        public int ExpectedOccupiedSpaces => ExpectedVehicles.Count;
        public int ExpectedAvailableSpaces => ExpectedCapacity - ExpectedOccupiedSpaces;
    }

    public static GarageFixtureInfo CreateGarageWithVehicles(string name = "Test garage", int capacity = 6, FixtureKind kind = FixtureKind.Minimal, bool variedCounts = false)
    {
        var vehicles = CreateFixtureVehicles(kind, variedCounts);

        if (capacity < vehicles.Count)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must fit all fixture vehicles.");

        Garage<IVehicle> garage = CreateGarage(name, capacity);
        List<(int SpotNumber, IVehicle Vehicle)> expectedVehicles = [];

        for (int index = 0; index < vehicles.Count; index++)
        {
            IVehicle vehicle = vehicles[index];
            int spotNumber = index + 1;

            var result = garage.ParkVehicle(vehicle, spotNumber);

            if (!result.Success)
                throw new InvalidOperationException($"Fixture setup failed: {result.Message}");

            expectedVehicles.Add((spotNumber, vehicle));
        }

        return new GarageFixtureInfo(garage, name.Trim(), capacity, expectedVehicles.AsReadOnly());
    }

    private static IReadOnlyList<IVehicle> CreateFixtureVehicles(FixtureKind kind, bool variedCounts)
    {
        if (!variedCounts)
            return VehicleFixtures.CreateAll(kind);

        List<IVehicle> vehicles = [];

        AddVehicles(1, index => VehicleFixtures.CreateCar(kind, registrationNumber: $"CAR{index:000}"));
        AddVehicles(2, index => VehicleFixtures.CreateMotorcycle(kind, registrationNumber: $"MCY{index:000}"));
        AddVehicles(3, index => VehicleFixtures.CreateBus(kind, registrationNumber: $"BUS{index:000}"));
        AddVehicles(4, index => VehicleFixtures.CreateAirplane(kind, registrationNumber: $"SE-AA{(char)('A' + index - 1)}"));
        AddVehicles(5, index => VehicleFixtures.CreateBoat(kind, registrationNumber: $"SFB-{2000 + index}"));
        AddVehicles(6, index => VehicleFixtures.CreateTruck(kind, registrationNumber: $"TRK{index:000}"));

        return vehicles.AsReadOnly();

        void AddVehicles(int count, Func<int, IVehicle> factory)
        {
            for (int index = 1; index <= count; index++)
                vehicles.Add(factory(index));
        }
    }

    public static IReadOnlyDictionary<string, int> VariedTypeCounts { get; } =
    new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(
        new Dictionary<string, int>
        {
            ["Car"] = 1,
            ["Motorcycle"] = 2,
            ["Bus"] = 3,
            ["Airplane"] = 4,
            ["Boat"] = 5,
            ["Truck"] = 6
        });

    public static IReadOnlyList<string> ExpectedTypeOrder { get; } =
    Array.AsReadOnly(new[] { "Airplane", "Boat", "Bus", "Car", "Motorcycle", "Truck" });
}