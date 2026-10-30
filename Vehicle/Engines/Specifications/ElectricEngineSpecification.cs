namespace Garage.Engines.Specifications;

public class ElectricEngineSpecification : EngineSpecification
{
    public required double PowerKw { get; init; }
    public required double Voltage { get; init; }
}
