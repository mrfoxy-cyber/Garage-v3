using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class MotorcycleSpecification : VehicleSpecification
{
    public MotorcycleSpecification(
        MotorcycleType? motorcycleType = null,
        bool? hasSidecar = null,
        int? numberOfWheels = null,
        int? passengerCapacity = null,
        int? driverSeats = null)
    {
        if (motorcycleType is { } type && !Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(motorcycleType),
                "Choose a supported motorcycle type.");
        }

        if (numberOfWheels < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfWheels),
                "Number of wheels cannot be negative.");
        }

        if (passengerCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(passengerCapacity),
                "Passenger capacity cannot be negative.");
        }

        if (driverSeats < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(driverSeats),
                "Number of driver seats cannot be negative.");
        }

        MotorcycleType = motorcycleType;
        HasSidecar = hasSidecar;
        NumberOfWheels = numberOfWheels;
        PassengerCapacity = passengerCapacity;
        DriverSeats = driverSeats;
    }

    public MotorcycleType? MotorcycleType { get; }
    public bool? HasSidecar { get; }
    public int? NumberOfWheels { get; }
    public int? PassengerCapacity { get; }
    public int? DriverSeats { get; }
}