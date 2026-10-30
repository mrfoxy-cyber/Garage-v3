namespace Garage.Engines.Specifications;

public class SteamEngineSpecification : ExternalCombustionEngineSpecification
{
    public required int HorsePower { get; init; }
    public required double BoilerPressureBar { get; init; }
}
