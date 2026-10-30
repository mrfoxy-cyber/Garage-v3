using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Turbine;

public class TurbojetEngine : TurbineEngine
{
    public required TurbineEngineSpecification Specification { get; init; }
    protected override TurbineEngineSpecification TurbineSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;
}
