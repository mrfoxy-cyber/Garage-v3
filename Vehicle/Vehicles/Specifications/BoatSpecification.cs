using Garage.Vehicles.Components;
using Garage.Vehicles.Types;

namespace Garage.Vehicles.Specifications;

public class BoatSpecification : VehicleSpecification
{
    public BoatSpecification(
        int? passengerCapacity = null,
        int? driverSeats = null,
        Cabin? cabin = null,
        BoatType? boatType = null,
        double? draftMeters = null)
    {
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

        if (boatType is { } type && !Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(boatType),
                "Choose a supported boat type.");
        }

        if (draftMeters is { } draft &&
            (!double.IsFinite(draft) || draft < 0))
        {
            throw new ArgumentOutOfRangeException(
                nameof(draftMeters),
                "Draft must be a finite, non-negative number.");
        }

        if (cabin is not null &&
            passengerCapacity is { } capacity &&
            cabin.PassengerSeatCount > capacity)
        {
            throw new ArgumentException(
                "Cabin passenger seats cannot exceed " +
                "the boat's passenger capacity.",
                nameof(cabin));
        }

        PassengerCapacity = passengerCapacity;
        DriverSeats = driverSeats;
        Cabin = cabin;
        BoatType = boatType;
        DraftMeters = draftMeters;
    }

    public BoatType? BoatType { get; }
    public double? DraftMeters { get; }
    public int? PassengerCapacity { get; }
    public int? DriverSeats { get; }
    public Cabin? Cabin { get; }
}