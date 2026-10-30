namespace Garage.Engines.Specifications;

public class RotaryEngineSpecification : InternalCombustionEngineSpecification
{
    public required int HorsePower { get; init; }
    public required int DisplacementCc { get; init; }
    public required int RotorCount { get; init; }
}
