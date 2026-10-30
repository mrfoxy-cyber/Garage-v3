using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Turbine;

public class TurbofanEngine : TurbineEngine
{
    public required TurbofanEngineSpecification Specification { get; init; }
    protected override TurbineEngineSpecification TurbineSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;
}
