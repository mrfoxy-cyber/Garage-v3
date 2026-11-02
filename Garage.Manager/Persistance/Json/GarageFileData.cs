using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Manager.Persistence.Json;

public sealed record GarageFileData
{
    public required int Version { get; init; }
    public required string Name { get; init; }
    public required int Capacity { get; init; }
    public required List<VehicleFileData> Vehicles { get; init; }
}

public sealed record VehicleFileData
{
    public required string VehicleType { get; init; }
    public required string RegistrationNumber { get; init; }
    public required CountryCode CountryCode { get; init; }
    public VehicleColor? Color { get; init; }
    public int? NumberOfWheels { get; init; }
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public int? ManufacturingYear { get; init; }
}
