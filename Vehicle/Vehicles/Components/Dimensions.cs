namespace Garage.Vehicles.Components;

public sealed record Dimensions
{
    public double LengthMeters { get; }
    public double WidthMeters { get; }
    public double HeightMeters { get; }

    public Dimensions(double lengthMeters, double widthMeters, double heightMeters)
    {
        LengthMeters = RequirePositiveFinite(lengthMeters, nameof(lengthMeters));
        WidthMeters = RequirePositiveFinite(widthMeters, nameof(widthMeters));
        HeightMeters = RequirePositiveFinite(heightMeters, nameof(heightMeters));
    }

    private static double RequirePositiveFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "A dimension must be a positive, finite number of meters.");
        }

        return value;
    }
}
