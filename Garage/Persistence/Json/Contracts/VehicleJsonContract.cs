using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Persistence.Json.Contracts;

internal sealed class VehicleJsonContract
{
    public required string Type { get; init; }
    public required string RegistrationNumber { get; init; }
    public required CountryCode CountryCode { get; init; }
    public required string Manufacturer { get; init; }
    public required string Model { get; init; }
    public required int ManufacturingYear { get; init; }
    public required VehicleColor Color { get; init; }
    public required double WeightKg { get; init; }
    public required double LengthMeters { get; init; }
    public required double WidthMeters { get; init; }
    public required double HeightMeters { get; init; }
    public List<EngineJsonContract> Engines { get; init; } = [];

    public string? Vin { get; init; }
    public string? Hin { get; init; }
    public string? AirframeSerialNumber { get; init; }

    public BodyType? BodyType { get; init; }
    public MotorcycleType? MotorcycleType { get; init; }
    public BoatType? BoatType { get; init; }

    public int? NumberOfDoors { get; init; }
    public int? NumberOfSeats { get; init; }
    public int? NumberOfWheels { get; init; }
    public int? NumberOfAxles { get; init; }

    public bool? HasSidecar { get; init; }
    public int? PassengerCapacity { get; init; }
    public int? DriverSeats { get; init; }

    public int? PassengerSeatCount { get; init; }
    public int? StandingPassengerCapacity { get; init; }
    public int? DriverSeatCount { get; init; }

    public double? MaximumLoadKg { get; init; }

    public double? DraftMeters { get; init; }
    public bool? HasCabin { get; init; }
    public int? CabinPassengerSeatCount { get; init; }
    public int? CabinCrewSeatCount { get; init; }

    public double? WingspanMeters { get; init; }
    public int? CrewSeatCount { get; init; }
    public int? PilotSeatCount { get; init; }
    public bool? HasDualControls { get; init; }
    public double? MaximumAltitudeMeters { get; init; }
}
