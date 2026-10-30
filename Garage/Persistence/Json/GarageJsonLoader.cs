using System.Text.Json;
using System.Text.Json.Serialization;
using Garage.Specifications;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Persistence.Json;

public static class GarageJsonLoader
{
public static VehicleGarage Load(string configurationPath, string? placementPath = null)
    {
        GarageSpecification configuration = ReadGarageSpecification(configurationPath);
        if (string.IsNullOrWhiteSpace(configuration.GarageName))
            throw new InvalidDataException("Garage configuration needs a garageName.");

        if (configuration.Capacity < 1 || configuration.Capacity > VehicleGarage.MaximumGarageCapacity)
            throw new InvalidDataException($"Capacity must be between 1 and {VehicleGarage.MaximumGarageCapacity}.");

        IReadOnlyList<SpotSpecification> overrides = configuration.Spots ?? [];
        if (overrides.Count > 0 && configuration.DefaultSpotSize is null)
            throw new InvalidDataException("A defaultSpotSize is required when spots have custom sizes.");

        VehicleGarage garage;
        if (configuration.DefaultSpotSize is null)
        {
            garage = new VehicleGarage(configuration.GarageName.Trim(), configuration.Capacity);
        }
        else
        {
            Dimensions defaultSize = ToDimensions(configuration.DefaultSpotSize, "defaultSpotSize");
            List<Dimensions> sizes = Enumerable.Repeat(defaultSize, configuration.Capacity).ToList();
            HashSet<int> usedNumbers = [];
            foreach (SpotSpecification spot in overrides)
            {
                if (spot is null || spot.SpotNumber < 1 || spot.SpotNumber > configuration.Capacity ||
                    !usedNumbers.Add(spot.SpotNumber) || spot.Size is null)
                    throw new InvalidDataException("Each custom spot needs a unique spotNumber within capacity and a size.");

                sizes[spot.SpotNumber - 1] = ToDimensions(spot.Size, $"spot {spot.SpotNumber}");
            }

            garage = new VehicleGarage(configuration.GarageName.Trim(), sizes);
        }

        if (placementPath is not null)
            ApplyPlacements(garage, placementPath);

        return garage;
    }

    private static void ApplyPlacements(VehicleGarage garage, string placementPath)
    {
        GaragePlacementSpecification placement = ReadPlacementSpecification(placementPath);
        if (!string.Equals(garage.GarageName, placement.GarageName?.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Placement garageName does not match the garage configuration.");

        if (string.IsNullOrWhiteSpace(placement.VehiclesFile) || placement.Assignments is null)
            throw new InvalidDataException("Placement needs vehiclesFile and assignments.");

        string vehiclesPath = Path.GetFullPath(
            placement.VehiclesFile,
            Path.GetDirectoryName(Path.GetFullPath(placementPath))!);
        List<Vehicle> vehicles = VehicleJsonLoader.LoadVehicles(vehiclesPath);
        Dictionary<string, Vehicle> byRegistration = new(StringComparer.OrdinalIgnoreCase);
        foreach (Vehicle vehicle in vehicles)
        {
            if (!byRegistration.TryAdd(vehicle.RegistrationNumber, vehicle))
                throw new InvalidDataException($"Duplicate vehicle registration '{vehicle.RegistrationNumber}' in vehiclesFile.");
        }

        HashSet<int> usedSpots = [];
        HashSet<string> usedRegistrations = new(StringComparer.OrdinalIgnoreCase);
        foreach (SpotAssignment assignment in placement.Assignments)
        {
            if (assignment is null || assignment.SpotNumber < 1 || assignment.SpotNumber > garage.Capacity ||
                !usedSpots.Add(assignment.SpotNumber))
                throw new InvalidDataException("Assignments must use unique spot numbers within the garage capacity.");

            string registration = assignment.RegistrationNumber?.Trim() ?? "";
            if (!usedRegistrations.Add(registration) || !byRegistration.TryGetValue(registration, out Vehicle? vehicle))
                throw new InvalidDataException($"Registration '{registration}' is duplicate or absent from vehiclesFile.");

            var result = garage.Park(vehicle, assignment.SpotNumber);
            if (!result.Success)
                throw new InvalidDataException(result.Message);
        }
    }

    private static GarageSpecification ReadGarageSpecification(string path)
    {
        string json = ReadJson(path);
        return JsonSerializer.Deserialize<GarageSpecification>(json, JsonSettings.Options)
            ?? throw new InvalidDataException("The garage configuration JSON root cannot be null.");
    }

    private static GaragePlacementSpecification ReadPlacementSpecification(string path)
    {
        string json = ReadJson(path);
        return JsonSerializer.Deserialize<GaragePlacementSpecification>(json, JsonSettings.Options)
            ?? throw new InvalidDataException("The parking assignments JSON root cannot be null.");
    }

    private static string ReadJson(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("A JSON file path is required.", nameof(path));
        return File.ReadAllText(path);
    }

    private static Dimensions ToDimensions(SpotSizeSpecification size, string location)
    {
        try
        {
            return new Dimensions(size.LengthMeters, size.WidthMeters, size.HeightMeters);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new InvalidDataException($"Invalid dimensions for {location}: {exception.Message}", exception);
        }
    }
}
