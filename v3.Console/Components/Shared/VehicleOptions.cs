using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Terminal.Components.Shared;

public static class VehicleOptions
{
    public static IReadOnlyList<string> Types { get; } = Array.AsReadOnly(new[] { "Car", "Motorcycle", "Bus", "Boat", "Airplane" });
    public static IReadOnlyList<string> Countries { get; } = Array.AsReadOnly(Enum.GetNames<CountryCode>().Where(value => value != nameof(CountryCode.Unspecified)).ToArray());
    public static IReadOnlyList<string> Colors { get; } = Array.AsReadOnly(Enum.GetNames<VehicleColor>());
}
