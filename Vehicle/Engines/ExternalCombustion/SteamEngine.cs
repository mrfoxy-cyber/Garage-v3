using Garage.Engines.Specifications;

namespace Garage.Engines.ExternalCombustion;

public class SteamEngine : ExternalCombustionEngine
{
    public required SteamEngineSpecification Specification { get; init; }
    protected override ExternalCombustionEngineSpecification CombustionSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;
}
