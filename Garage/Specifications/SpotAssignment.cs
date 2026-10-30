namespace Garage.Specifications;

public sealed record SpotAssignment
{
    public int SpotNumber { get; init; }
    public string? RegistrationNumber { get; init; }
}
