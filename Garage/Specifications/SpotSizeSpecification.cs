namespace Garage.Specifications;

public sealed record SpotSizeSpecification
{
    public double LengthMeters { get; init; }
    public double WidthMeters { get; init; }
    public double HeightMeters { get; init; }
}
