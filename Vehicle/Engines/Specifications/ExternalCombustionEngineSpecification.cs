namespace Garage.Engines.Specifications;

public abstract class ExternalCombustionEngineSpecification : EngineSpecification
{
    public required FuelType FuelType { get; init; }
}
