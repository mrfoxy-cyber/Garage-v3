using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion;

public class RotaryEngine : InternalCombustionEngine
{
    public required RotaryEngineSpecification Specification { get; init; }
    protected override InternalCombustionEngineSpecification CombustionSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;
}
