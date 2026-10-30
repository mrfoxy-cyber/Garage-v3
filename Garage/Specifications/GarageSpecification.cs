namespace Garage.Specifications;

/// <summary>Describes the fixed garage layout independently of its occupants.</summary>
public sealed record GarageSpecification
{
    public string? GarageName { get; init; }
    public int Capacity { get; init; }
    public SpotSizeSpecification? DefaultSpotSize { get; init; }
    public IReadOnlyList<SpotSpecification>? Spots { get; init; }
}
