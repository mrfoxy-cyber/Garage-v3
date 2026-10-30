using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage_V3.queries;

public sealed record VehicleSearchCriteria
{
    public string? RegistrationNumber { get; init; }
    public CountryCode? CountryCode { get; init; }
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public int? ManufacturingYear { get; init; }
    public VehicleColor? Color { get; init; }
    public int? NumberOfWheels { get; init; }
    public string? VehicleType { get; init; }
}