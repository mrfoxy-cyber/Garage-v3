using Garage.Engines.InternalCombustion;
using Garage.Engines.Specifications;

namespace Garage.Engines.InternalCombustion.Turbine;

public abstract class TurbineEngine : InternalCombustionEngine
{
    protected abstract TurbineEngineSpecification TurbineSpecification { get; }
    protected override InternalCombustionEngineSpecification CombustionSpecification => TurbineSpecification;
}
