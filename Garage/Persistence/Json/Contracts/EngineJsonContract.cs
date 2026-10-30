using Garage.Engines;

namespace Garage.Persistence.Json.Contracts;

internal sealed class EngineJsonContract
{
    public required string Type { get; init; }
    public required string SerialNumber { get; init; }
    public required string Model { get; init; }

    public FuelType? FuelType { get; init; }
    public int? HorsePower { get; init; }
    public int? DisplacementCc { get; init; }
    public int? CylinderCount { get; init; }
    public int? RotorCount { get; init; }
    public double? PowerKw { get; init; }
    public double? Voltage { get; init; }
    public double? BoilerPressureBar { get; init; }
    public double? ThrustKn { get; init; }
    public double? BypassRatio { get; init; }
    public int? ShaftHorsePower { get; init; }
}
