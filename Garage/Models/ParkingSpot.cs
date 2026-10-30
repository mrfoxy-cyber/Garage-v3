using Garage.Vehicles;
using Garage.Vehicles.Components;

namespace Garage.Models;

internal sealed class ParkingSpot
{
    public ParkingSpot(int number, Dimensions? dimensions)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        Number = number;
        Dimensions = dimensions;
    }

    public int Number { get; }
    public Dimensions? Dimensions { get; }
    public Vehicle? Vehicle { get; private set; }

    public bool Fits(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (Dimensions is null)
            return true;
        Dimensions size = vehicle.SpecificationDetails.Dimensions;
        return size.LengthMeters <= Dimensions.LengthMeters &&
               size.WidthMeters <= Dimensions.WidthMeters &&
               size.HeightMeters <= Dimensions.HeightMeters;
    }

    public void Park(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (Vehicle is not null || !Fits(vehicle))
            throw new InvalidOperationException("The spot must be empty and large enough for the vehicle.");
        Vehicle = vehicle;
    }

    public void Clear() => Vehicle = null;
}
