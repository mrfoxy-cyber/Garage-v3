namespace Garage.Engines.Specifications;

public class TurbineEngineSpecification : InternalCombustionEngineSpecification
{
    public required double ThrustKn { get; init; }
}
