using System.Text.Json;
using Garage.Persistence.Json.Contracts;
using Garage.Vehicles;

namespace Garage.Persistence.Json;

public static class VehicleJsonLoader
{
    /// <summary>Reads one vehicle object. Relative paths use the current working directory.</summary>
    public static Vehicle LoadVehicle(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        VehicleJsonContract specification = JsonSerializer.Deserialize<VehicleJsonContract>(
            File.ReadAllText(filePath), JsonSettings.Options)
            ?? throw new InvalidDataException("Vehicle JSON must be an object and cannot be null.");
        return VehicleJsonMapper.Create(specification);
    }

    /// <summary>Reads a catalog array. Relative sample paths use the application's Data folder.</summary>
    public static List<Vehicle> LoadVehicles(string? filePath = null)
    {
        filePath ??= "vehicles.json";
        if (!Path.IsPathRooted(filePath))
            filePath = Path.Combine(AppContext.BaseDirectory, "Data", filePath);

        List<VehicleJsonContract> specifications = JsonSerializer.Deserialize<List<VehicleJsonContract>>(
            File.ReadAllText(filePath), JsonSettings.Options)
            ?? throw new InvalidDataException("Vehicle data must be a JSON array and cannot be null.");

        List<Vehicle> vehicles = [];
        for (int index = 0; index < specifications.Count; index++)
        {
            VehicleJsonContract specification = specifications[index]
                ?? throw new InvalidDataException($"Vehicle at position {index + 1} cannot be null.");
            vehicles.Add(VehicleJsonMapper.Create(specification));
        }
        return vehicles;
    }
}
