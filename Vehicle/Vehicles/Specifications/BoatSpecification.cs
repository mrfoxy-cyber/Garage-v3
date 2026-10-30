using Garage.Vehicles.Components;
using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class BoatSpecification : VehicleSpecification
{
    public BoatSpecification(
        int passengerCapacity,
        int driverSeats,
        Cabin? cabin)
    {
        if (cabin is not null &&
            cabin.PassengerSeatCount > passengerCapacity)
        {
            throw new ArgumentException(
                "Cabin passenger seats cannot exceed " +
                "the boat's passenger capacity.",
                nameof(cabin));
        }

        PassengerCapacity = passengerCapacity;
        DriverSeats = driverSeats;
        Cabin = cabin;
    }

    public required BoatType BoatType { get; init; }
    public required double DraftMeters { get; init; }
    public int PassengerCapacity { get; }
    public int DriverSeats { get; }
    public Cabin? Cabin { get; }
}
