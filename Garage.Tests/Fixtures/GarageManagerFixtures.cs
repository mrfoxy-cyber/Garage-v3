using Garage.Manager;
using Garage.Manager.Interfaces;
using Vehicles.Interfaces;

namespace Garage.Tests.Fixtures;

internal enum ManagerGarageKind
{
    Mixed,
    Car,
    Motorcycle,
    Bus,
    Airplane,
    Boat,
    Truck,
    Empty
}

internal sealed record ManagerVehicleFixtureInfo(
    string GarageId,
    string GarageName,
    int SpotNumber,
    IVehicle Vehicle);

internal sealed record ManagerGarageFixtureInfo(
    ManagerGarageKind Kind,
    string GarageId,
    string ExpectedName,
    int ExpectedCapacity,
    IReadOnlyList<(int SpotNumber, IVehicle Vehicle)> ExpectedVehicles)
{
    public int ExpectedOccupiedSpaces => ExpectedVehicles.Count;
    public int ExpectedAvailableSpaces => ExpectedCapacity - ExpectedOccupiedSpaces;

    public IReadOnlyDictionary<string, int> ExpectedTypeCounts =>
        ExpectedVehicles
            .GroupBy(entry => entry.Vehicle.GetType().Name)
            .ToDictionary(group => group.Key, group => group.Count());
}

internal sealed record GarageManagerFixtureInfo(
    IGarageManager Manager,
    IReadOnlyList<ManagerGarageFixtureInfo> Garages)
{
    public int ExpectedGarageCount => Garages.Count;
    public int ExpectedVehicleCount => Garages.Sum(garage => garage.ExpectedOccupiedSpaces);

    public ManagerGarageFixtureInfo GetGarage(ManagerGarageKind kind)
    {
        return Garages.Single(garage => garage.Kind == kind);
    }

    public IReadOnlyList<ManagerVehicleFixtureInfo> ExpectedLocations =>
        Garages.SelectMany(garage => garage.ExpectedVehicles.Select(entry =>
            new ManagerVehicleFixtureInfo(
                garage.GarageId,
                garage.ExpectedName,
                entry.SpotNumber,
                entry.Vehicle)))
            .ToList()
            .AsReadOnly();
}

internal static class GarageManagerFixtures
{
    private static readonly ManagerGarageKind[] VehicleKinds =
    [
        ManagerGarageKind.Car,
        ManagerGarageKind.Motorcycle,
        ManagerGarageKind.Bus,
        ManagerGarageKind.Airplane,
        ManagerGarageKind.Boat,
        ManagerGarageKind.Truck
    ];

    private static readonly (ManagerGarageKind Kind, string Name, int Capacity, int VehicleCount)[] Definitions =
    [
        (ManagerGarageKind.Mixed, "Mixed garage", 12, 6),
        (ManagerGarageKind.Car, "Car garage", 5, 1),
        (ManagerGarageKind.Motorcycle, "Motorcycle garage", 6, 2),
        (ManagerGarageKind.Bus, "Bus garage", 7, 3),
        (ManagerGarageKind.Airplane, "Airplane garage", 8, 4),
        (ManagerGarageKind.Boat, "Boat garage", 9, 5),
        (ManagerGarageKind.Truck, "Truck garage", 10, 6),
        (ManagerGarageKind.Empty, "Empty garage", 5, 0)
    ];

    public static IGarageManager CreateManager()
    {
        return new GarageManager();
    }

    public static GarageManagerFixtureInfo CreateManagerWithGarages(FixtureKind kind = FixtureKind.Minimal)
    {
        IGarageManager manager = CreateManager();
        List<ManagerGarageFixtureInfo> garages = [];

        for (int group = 0; group < Definitions.Length; group++)
        {
            var definition = Definitions[group];
            var vehicles = CreateVehicles(definition.Kind, definition.VehicleCount, group, kind);

            var garage = CreateGarage(
                manager,
                definition.Kind,
                definition.Name,
                definition.Capacity,
                vehicles);

            garages.Add(garage);
        }

        return new GarageManagerFixtureInfo(manager, garages.AsReadOnly());
    }

    private static IReadOnlyList<IVehicle> CreateVehicles(ManagerGarageKind garageKind, int count, int group, FixtureKind kind)
    {
        List<IVehicle> vehicles = [];

        if (garageKind == ManagerGarageKind.Empty)
            return vehicles.AsReadOnly();

        if (garageKind == ManagerGarageKind.Mixed)
        {
            foreach (ManagerGarageKind vehicleKind in VehicleKinds)
                vehicles.Add(CreateVehicle(vehicleKind, group, 1, kind));
        }
        else
        {
            for (int index = 1; index <= count; index++)
                vehicles.Add(CreateVehicle(garageKind, group, index, kind));
        }

        return vehicles.AsReadOnly();
    }

    private static IVehicle CreateVehicle(ManagerGarageKind type, int group, int index, FixtureKind kind)
    {
        int number = group * 100 + index;

        return type switch
        {
            ManagerGarageKind.Car => VehicleFixtures.CreateCar(
                kind, registrationNumber: $"CAR{number:000}"),

            ManagerGarageKind.Motorcycle => VehicleFixtures.CreateMotorcycle(
                kind, registrationNumber: $"MCY{number:000}"),

            ManagerGarageKind.Bus => VehicleFixtures.CreateBus(
                kind, registrationNumber: $"BUS{number:000}"),

            ManagerGarageKind.Airplane => VehicleFixtures.CreateAirplane(
                kind, registrationNumber: $"SE-{(char)('A' + group)}A{(char)('A' + index)}"),

            ManagerGarageKind.Boat => VehicleFixtures.CreateBoat(
                kind, registrationNumber: $"SFB-{2000 + number}"),

            ManagerGarageKind.Truck => VehicleFixtures.CreateTruck(
                kind, registrationNumber: $"TRK{number:000}"),

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private static ManagerGarageFixtureInfo CreateGarage(IGarageManager manager, ManagerGarageKind kind, string name, int capacity, IReadOnlyList<IVehicle> vehicles)
    {
        var (garageId, result) = manager.CreateGarage(name, capacity, vehicles);

        if (!result.Success || string.IsNullOrWhiteSpace(garageId))
            throw new InvalidOperationException($"Manager fixture setup failed: {result.Message}");

        List<(int SpotNumber, IVehicle Vehicle)> expectedVehicles = [];

        for (int index = 0; index < vehicles.Count; index++)
            expectedVehicles.Add((index + 1, vehicles[index]));

        return new ManagerGarageFixtureInfo(
            kind,
            garageId,
            name,
            capacity,
            expectedVehicles.AsReadOnly());
    }
}