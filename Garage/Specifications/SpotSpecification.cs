namespace Garage.Specifications;

public sealed record SpotSpecification
{
    public int SpotNumber { get; init; }
    public SpotSizeSpecification? Size { get; init; }
}
