using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class MotorcycleSpecification : VehicleSpecification
{
    public required MotorcycleType MotorcycleType { get; init; }
    public required bool HasSidecar { get; init; }
    public required int NumberOfWheels { get; init; }
    public required int PassengerCapacity { get; init; }
    public required int DriverSeats { get; init; }
}
