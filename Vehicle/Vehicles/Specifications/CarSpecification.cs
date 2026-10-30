using Garage.Vehicles.Components;
using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class CarSpecification : VehicleSpecification
{
    public required BodyType BodyType { get; init; }
    public required int NumberOfDoors { get; init; }
    public required int NumberOfWheels { get; init; }
    public required Cabin Cabin { get; init; }
}
