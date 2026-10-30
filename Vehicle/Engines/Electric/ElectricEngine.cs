using Garage.Engines;
using Garage.Engines.Specifications;

namespace Garage.Engines.Electric;

public abstract class ElectricEngine : Engine
{
    public required ElectricEngineSpecification Specification { get; init; }
    public override EngineSpecification SpecificationDetails => Specification;
}
