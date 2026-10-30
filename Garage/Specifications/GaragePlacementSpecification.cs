namespace Garage.Specifications;

/// <summary>Describes occupants independently of the garage layout.</summary>
public sealed record GaragePlacementSpecification
{
    public string? GarageName { get; init; }
    public string? VehiclesFile { get; init; }
    public IReadOnlyList<SpotAssignment>? Assignments { get; init; }
}
