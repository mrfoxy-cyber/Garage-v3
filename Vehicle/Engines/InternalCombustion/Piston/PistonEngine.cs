using Garage.Engines.InternalCombustion;
using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Piston;

public abstract class PistonEngine : InternalCombustionEngine
{
    public required PistonEngineSpecification Specification { get; init; }
    protected override InternalCombustionEngineSpecification CombustionSpecification => Specification;
    public override EngineSpecification SpecificationDetails => Specification;

    public override double? CylinderVolumeCc =>
        Specification.CylinderVolumeCc;
}
