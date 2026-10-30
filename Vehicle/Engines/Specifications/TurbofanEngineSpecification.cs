namespace Garage.Engines.Specifications;

public class TurbofanEngineSpecification : TurbineEngineSpecification
{
    public required double BypassRatio { get; init; }
}
