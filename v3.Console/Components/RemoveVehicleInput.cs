using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class RemoveVehicleInput : FormComponent
{
    public TextField Registration { get; }
    public event Action<string>? LookupRequested;

    public RemoveVehicleInput(GarageTheme theme) : base(theme)
    {
        Registration = AddTextInput("Registration number");
        AddAction("_Find vehicle to remove", Submit);
    }

    public void SetRegistration(string registration)
    {
        Registration.Text = registration;
        ShowError(null);
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
        LookupRequested?.Invoke(registration);
    }
}
