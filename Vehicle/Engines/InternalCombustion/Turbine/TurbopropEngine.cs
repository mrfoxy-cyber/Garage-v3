using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Turbine;

public class TurbopropEngine : TurbineEngine
{
    public required TurbopropEngineSpecification Specification { get; init; }
    protected override TurbineEngineSpecification TurbineSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;
}
