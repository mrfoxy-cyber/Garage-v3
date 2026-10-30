namespace Garage.Engines.Specifications;

public abstract class InternalCombustionEngineSpecification : EngineSpecification
{
    public required FuelType FuelType { get; init; }
}
