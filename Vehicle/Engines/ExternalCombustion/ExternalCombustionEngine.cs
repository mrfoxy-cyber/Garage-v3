using Garage.Engines;
using Garage.Engines.Specifications;

namespace Garage.Engines.ExternalCombustion;

public abstract class ExternalCombustionEngine : Engine
{
    protected abstract ExternalCombustionEngineSpecification CombustionSpecification { get; }
    public override FuelType? FuelType => CombustionSpecification.FuelType;
}
