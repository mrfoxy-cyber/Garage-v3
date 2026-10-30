using Garage.Vehicles.Components;
using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class CarSpecification : VehicleSpecification
{
    public CarSpecification(
        BodyType? bodyType = null,
        int? numberOfDoors = null,
        int? numberOfWheels = null,
        Cabin? cabin = null)
    {
        if (bodyType is { } type && !Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(bodyType),
                "Choose a supported body type.");
        }

        if (numberOfDoors < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfDoors),
                "Number of doors cannot be negative.");
        }

        if (numberOfWheels < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfWheels),
                "Number of wheels cannot be negative.");
        }

        BodyType = bodyType;
        NumberOfDoors = numberOfDoors;
        NumberOfWheels = numberOfWheels;
        Cabin = cabin;
    }

    public BodyType? BodyType { get; }
    public int? NumberOfDoors { get; }
    public int? NumberOfWheels { get; }
    public Cabin? Cabin { get; }
}