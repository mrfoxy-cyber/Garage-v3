using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class VehicleDetailsInput : FormComponent
{
    public ChoiceInput Color { get; }
    public TextField Wheels { get; }
    public TextField Manufacturer { get; }

    public TextField Model { get; }

    public TextField ManufacturingYear { get; }

    public VehicleDetailsInput(GarageTheme theme) : base(theme)
    {
        Color = AddChoiceInput("Colour", ["Unknown", .. VehicleOptions.Colors], "Unknown");
        Wheels = AddTextInput("Wheel count (blank = unknown)");
        Manufacturer = AddTextInput("Manufacturer (blank = unknown)");
        Model = AddTextInput("Model (blank = unknown)");
        ManufacturingYear = AddTextInput("Manufacturing year (blank = unknown)");
    }



    public void Reset()
    {
        Wheels.Text = string.Empty;
        Color.SetValue("Unknown");
        Manufacturer.Text = string.Empty;
        Model.Text = string.Empty;
        ManufacturingYear.Text = string.Empty;
        ShowError(null);
    }
}
