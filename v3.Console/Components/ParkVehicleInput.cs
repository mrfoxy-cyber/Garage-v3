using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class ParkVehicleInput : FormComponent
{

    private readonly Label registrationHint;
    public ChoiceInput VehicleType { get; }
    public ChoiceInput Country { get; }
    public ChoiceInput Color { get; }
    public TextField Registration { get; }
    public TextField Spot { get; }
    public TextField Wheels { get; }

    public event Action<ParkVehicleRequest>? ParkRequested;

    public ParkVehicleInput(GarageTheme theme) : base(theme)
    {
        VehicleType = AddChoiceInput("Vehicle type", VehicleOptions.Types);
        Registration = AddTextInput("Registration number");
        registrationHint = AddTextBlock(string.Empty);
        Country = AddChoiceInput("Country", VehicleOptions.Countries, "SWE");
        Color = AddChoiceInput("Colour", ["Unknown", .. VehicleOptions.Colors], "Unknown");
        Wheels = AddTextInput("Wheel count (blank = unknown)");
        Spot = AddTextInput("Spot (blank = automatic)");

        AddAction("_Park vehicle", Submit);
    }

    public void SetAllowedTypes(IEnumerable<string> types) => VehicleType.SetOptions(types);

    public void Submit()
    {
        ShowError(null);
        string registration = Registration.Text.Trim();
        if (registration.Length == 0)
        {
            ShowError("Enter a registration number.");
            return;
        }
        if (VehicleType.Value.Length == 0)
        {
            ShowError("There are no vehicle types available for parking.");
            return;
        }

        int? spot = null;
        int? wheels = null;
        if (!string.IsNullOrWhiteSpace(Wheels.Text))
        {
            if (!int.TryParse(Wheels.Text, out int count) || count < 0)
            {
                ShowError("Wheel count must be a non-negative whole number, or blank.");
                return;
            }
            wheels = count;
        }
        if (!string.IsNullOrWhiteSpace(Spot.Text))
        {
            if (!int.TryParse(Spot.Text, out int number) || number < 1)
            {
                ShowError("Enter a positive whole-number spot, or leave it blank.");
                return;
            }
            spot = number;
        }

        VehicleColor? color = Color.Value == "Unknown" ? null : Enum.Parse<VehicleColor>(Color.Value);
        ParkRequested?.Invoke(new(VehicleType.Value, registration, Enum.Parse<CountryCode>(Country.Value), color, spot, wheels));

        VehicleType.ValueChanged += _ => UpdateRegistrationHint();
        Country.ValueChanged += _ => UpdateRegistrationHint();

        UpdateRegistrationHint();


    }

    private void UpdateRegistrationHint()
    {
        registrationHint.Text = (VehicleType.Value, Country.Value) switch
        {
            ("Boat", "SWE") =>
                "Examples: SABC, SAM-2000 or 7SA-2000.",

            ("Airplane", "SWE") =>
                "Example: SE-ABC. Include the hyphen.",

            ("Car" or "Motorcycle" or "Bus", "SWE") =>
                "Examples: ABC123 or ABC12A.",

            _ =>
                "No format example available for this selection."
        };

        RequestReflow();
    }

    public void Reset()
    {
        Registration.Text = string.Empty;
        Spot.Text = string.Empty;
        Wheels.Text = string.Empty;
        Color.SetValue("Unknown");
        ShowError(null);
    }
}
