using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class FindVehicleInput : FormComponent
{
    public TextField Registration { get; }
    public event Action<string>? FindRequested;

    public FindVehicleInput(GarageTheme theme) : base(theme)
    {
        Registration = AddTextInput("Registration number");
        AddAction("_Find vehicle", Submit);
    }

    public void Submit()
    {
        ShowError(null);
        string registration = Registration.Text.Trim();
        if (registration.Length == 0)
        {
            ShowError("Enter a registration number.");
            return;
        }
        FindRequested?.Invoke(registration);
    }
}
