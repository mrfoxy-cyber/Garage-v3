using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class VehicleIdentityInput : FormComponent
{

    private readonly Label registrationHint;
    public ChoiceInput VehicleType { get; }
    public ChoiceInput Country { get; }
    public TextField Registration { get; }

    public VehicleIdentityInput(GarageTheme theme) : base(theme)
    {
        VehicleType = AddChoiceInput("Vehicle type", VehicleOptions.Types);
        Registration = AddTextInput("Registration number");
        registrationHint = AddTextBlock(string.Empty);
        Country = AddChoiceInput("Country", VehicleOptions.Countries, "SWE");

        VehicleType.ValueChanged += _ => UpdateRegistrationHint();
        Country.ValueChanged += _ => UpdateRegistrationHint();
    }

    public void SetAllowedTypes(IEnumerable<string> types) => VehicleType.SetOptions(types);

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
        ShowError(null);
    }
}
