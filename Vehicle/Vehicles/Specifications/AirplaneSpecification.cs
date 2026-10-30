using Garage.Vehicles.Components;

namespace Garage.Vehicles.Specifications;

public class AirplaneSpecification : VehicleSpecification
{
    public AirplaneSpecification(
        Cabin? cabin = null,
        Cockpit? cockpit = null,
        double? wingspanMeters = null,
        double? maximumAltitudeMeters = null,
        int? numberOfWheels = null)
    {
        if (wingspanMeters is { } wingspan &&
            (!double.IsFinite(wingspan) || wingspan <= 0))
        {
            throw new ArgumentOutOfRangeException(
                nameof(wingspanMeters),
                "Wingspan must be a finite, positive number.");
        }

        if (maximumAltitudeMeters is { } altitude &&
            (!double.IsFinite(altitude) || altitude < 0))
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumAltitudeMeters),
                "Maximum altitude must be a finite, non-negative number.");
        }

        if (numberOfWheels < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfWheels),
                "Number of wheels cannot be negative.");
        }

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
        WingspanMeters = wingspanMeters;
        MaximumAltitudeMeters = maximumAltitudeMeters;
        NumberOfWheels = numberOfWheels;
    }

    public double? WingspanMeters { get; }
    public double? MaximumAltitudeMeters { get; }
    public int? NumberOfWheels { get; }
    public Cabin? Cabin { get; }
    public Cockpit? Cockpit { get; }
}