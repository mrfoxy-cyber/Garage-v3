using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Queries;

public sealed class VehicleSearchCriteria
{
    public string? RegistrationNumber { get; init; }
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }

    public int? ManufacturingYear { get; init; }
    public VehicleColor? Color { get; init; }

    public IReadOnlyList<Type> VehicleTypes { get; init; } = [];
    public int? NumberOfWheels { get; init; }
    public int? NumberOfEngines { get; init; }
    public double? CylinderVolumeCc { get; init; }
    public Garage.Engines.FuelType? FuelType { get; init; }
    public int? NumberOfSeats { get; init; }
    public double? LengthMeters { get; init; }

    public BodyType? BodyType { get; init; }
    public MotorcycleType? MotorcycleType { get; init; }
    public bool? HasSidecar { get; init; }
    public BoatType? BoatType { get; init; }
    public double? DraftMeters { get; init; }
    public double? WingspanMeters { get; init; }
    public double? MaximumAltitudeMeters { get; init; }
    public int? StandingPassengerCapacity { get; init; }
    public int? NumberOfAxles { get; init; }
    public double? MaximumLoadKg { get; init; }
}
