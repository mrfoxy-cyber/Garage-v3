using Garage.Engines;
using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion;

public abstract class InternalCombustionEngine : Engine
{
    protected abstract InternalCombustionEngineSpecification CombustionSpecification { get; }
    public override FuelType? FuelType => CombustionSpecification.FuelType;
}
