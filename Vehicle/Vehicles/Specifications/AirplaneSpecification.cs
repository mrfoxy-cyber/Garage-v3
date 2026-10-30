using Garage.Vehicles.Components;

namespace Garage.Vehicles.Specifications;

public class AirplaneSpecification : VehicleSpecification
{
    public AirplaneSpecification(
        Cabin cabin,
        Cockpit? cockpit)
    {
        if (cockpit is not null &&
            cockpit.HasDualControls &&
            cockpit.PilotSeats < 2)
        {
            throw new ArgumentException(
                "An airplane with dual controls must have " +
                "at least two pilot seats.",
                nameof(cockpit));
        }

        Cabin = cabin;
        Cockpit = cockpit;
    }

    public required double WingspanMeters { get; init; }
    public required double MaximumAltitudeMeters { get; init; }
    public required int NumberOfWheels { get; init; }
    public Cabin Cabin { get; }
    public Cockpit? Cockpit { get; }
}
