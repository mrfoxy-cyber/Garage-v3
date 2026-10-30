using Garage.Engines;
using Garage.Vehicles;

namespace Garage.Terminal.Forms;

/// <summary>Describes a form and calls its domain factory directly.</summary>
internal sealed partial class FactoryDefinition
{
    private readonly string methodName;

    private FactoryDefinition(string name, string methodName)
    {
        Name = name;
        this.methodName = methodName;
        Fields = BuildFields(methodName);
    }

    public string Name { get; }
    public List<FieldDefinition> Fields { get; }

    private static FieldDefinition Field(string name, Type type) =>
        new(name, FieldDefinition.FriendlyLabel(name), type,
            Initial: Default(name, type));

    private static string Default(string name, Type type) => name switch
    {
        "countryCode" => "SWE",
        "manufacturingYear" => DateTime.Now.Year.ToString(),
        "cabinPassengerSeatCount" or "cabinCrewSeatCount" => "0",
        _ => type == typeof(bool) ? "No"
            : type.IsEnum ? Enum.GetNames(type).ToList().First(value => value != "Unspecified")
            : ""
    };

    public static List<FactoryDefinition> Vehicles { get; } =
    [
        new("Car", "CreateCar"),
        new("Motorcycle", "CreateMotorcycle"),
        new("Airplane", "CreateAirplane"),
        new("Bus", "CreateBus"),
        new("Boat", "CreateBoat"),
        new("Truck", "CreateTruck")
    ];

    public static List<FactoryDefinition> Engines { get; } =
    [
        new("Inline", "CreateInlineEngine"),
        new("V", "CreateVEngine"),
        new("Flat", "CreateFlatEngine"),
        new("Rotary", "CreateRotaryEngine"),
        new("Electric Motor", "CreateElectricMotor"),
        new("Steam", "CreateSteamEngine"),
        new("Turbojet", "CreateTurbojetEngine"),
        new("Turbofan", "CreateTurbofanEngine"),
        new("Turboprop", "CreateTurbopropEngine"),
        new("Turboshaft", "CreateTurboshaftEngine")
    ];
}
