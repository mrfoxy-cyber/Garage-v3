using System.Reflection;
using Garage.Engines;
using Garage.Vehicles;

namespace Garage.Terminal.Presentation;

internal static class VehicleDetails
{
    public static string Summary(Vehicle vehicle) =>
        $"{vehicle.RegistrationNumber}  /  {vehicle.GetType().Name}  /  {vehicle.Manufacturer} {vehicle.Model}\n" +
        $"{vehicle.ManufacturingYear}  |  {vehicle.Color}  |  Registered in {vehicle.CountryCode}\n" +
        $"Size: {vehicle.SpecificationDetails.Dimensions.LengthMeters:0.##} x " +
        $"{vehicle.SpecificationDetails.Dimensions.WidthMeters:0.##} x " +
        $"{vehicle.SpecificationDetails.Dimensions.HeightMeters:0.##} m  |  " +
        $"{vehicle.SpecificationDetails.WeightKg:0.##} kg  |  {vehicle.SpecificationDetails.Engines.Count} engine(s)";

    public static string Full(Vehicle vehicle) => Summary(vehicle) + "\n\nVEHICLE SPECIFICATION\n" +
        Properties(vehicle.SpecificationDetails, ["Dimensions", "Engines"]) +
        "\n\nENGINES\n" + Engines(vehicle.SpecificationDetails.Engines);

    public static string Engines(List<Engine> engines)
    {
        List<string> descriptions = engines.Select((engine, index) =>
            $"{index + 1}. {engine.GetType().Name}\n{Properties(engine)}").ToList();
        return descriptions.Count == 0 ? "No engines added." : string.Join("\n\n", descriptions);
    }

    private static string Properties(object value, List<string>? excluded = null) => string.Join("\n",
        value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0 &&
                (excluded is null || !excluded.Contains(property.Name)))
            .Select(property => (property.Name, Value: property.GetValue(value)))
            .Where(property => property.Value is not null)
            .Select(property => $"{Forms.FieldDefinition.FriendlyLabel(property.Name)}: {property.Value}"));
}
