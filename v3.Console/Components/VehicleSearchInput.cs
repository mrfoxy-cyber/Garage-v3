using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Garage_V3.queries;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class VehicleSearchInput : FormComponent
{
    public TextField Registration { get; }
    public ChoiceInput VehicleType { get; }
    public ChoiceInput Country { get; }
    public ChoiceInput Color { get; }
    public TextField Wheels { get; }
    public TextField Manufacturer { get; }
    public TextField Model { get; }
    public TextField ManufacturingYear { get; }

    public event Action<VehicleSearchCriteria>? SearchRequested;
    public event Action? ClearRequested;

    public VehicleSearchInput(GarageTheme theme) : base(theme)
    {
        Registration = AddTextInput("Registration (blank = any)");
        VehicleType = AddChoiceInput("Vehicle type", ["Any", .. VehicleOptions.Types], "Any");
        Country = AddChoiceInput("Country", ["Any", .. VehicleOptions.Countries], "Any");
        Color = AddChoiceInput("Colour", ["Any", .. VehicleOptions.Colors], "Any");
        Wheels = AddTextInput("Wheel count (blank = any)");
        Manufacturer = AddTextInput("Manufacturer (blank = any)");
        Model = AddTextInput("Model (blank = any)");
        ManufacturingYear = AddTextInput("Year (blank = any)");
        AddAction("_Search", Submit);
        AddAction("_Clear filters", Clear);
    }

    public void Submit()
    {
        ShowError(null);
        if (!TryOptionalInteger(Wheels.Text, out int? wheels) || wheels < 0)
        {
            ShowError("Wheel count must be a non-negative whole number, or blank.");
            return;
        }
        if (!TryOptionalInteger(ManufacturingYear.Text, out int? year))
        {
            ShowError("Year must be a whole number, or blank.");
            return;
        }

        SearchRequested?.Invoke(new()
        {
            RegistrationNumber = Optional(Registration.Text),
            VehicleType = VehicleType.Value == "Any" ? null : VehicleType.Value,
            CountryCode = Country.Value == "Any" ? null : Enum.Parse<CountryCode>(Country.Value),
            Color = Color.Value == "Any" ? null : Enum.Parse<VehicleColor>(Color.Value),
            NumberOfWheels = wheels,
            Manufacturer = Optional(Manufacturer.Text),
            Model = Optional(Model.Text),
            ManufacturingYear = year
        });
    }

    public void Clear()
    {
        Registration.Text = Wheels.Text = Manufacturer.Text = Model.Text = ManufacturingYear.Text = string.Empty;
        VehicleType.SetValue("Any");
        Country.SetValue("Any");
        Color.SetValue("Any");
        ShowError(null);
        ClearRequested?.Invoke();
    }

    private static string? Optional(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    private static bool TryOptionalInteger(string text, out int? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(text))
            return true;
        if (!int.TryParse(text, out int number))
            return false;
        value = number;
        return true;
    }
}
