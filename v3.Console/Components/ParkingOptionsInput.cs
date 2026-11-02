using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class ParkingOptionsInput : FormComponent
{
    public TextField Spot { get; }

    public ParkingOptionsInput(GarageTheme theme) : base(theme)
    {
        Spot = AddTextInput("Spot (blank = automatic)");
    }



    public void Reset()
    {
        Spot.Text = string.Empty;
        ShowError(null);
    }
}
