using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Garage_V3;
using Terminal.Gui.Views;
using Vehicles.Interfaces;

namespace Garage.Terminal.Components;

public sealed class CreateGarageInput : FormComponent
{
    public TextField Name { get; }
    public TextField Capacity { get; }
    public event Action<string, int>? CreateRequested;

    public CreateGarageInput(GarageTheme theme) : base(theme)
    {
        Name = AddTextInput("Garage name");
        Capacity = AddTextInput("Capacity");
        AddAction("_Create garage", Submit);
    }

    public void Submit()
    {
        ShowError(null);
        string name = Name.Text.Trim();
        if (name.Length == 0)
        {
            ShowError("Enter a garage name.");
            Name.SetFocus();
            return;
        }
        if (!int.TryParse(Capacity.Text, out int capacity) || capacity < 1 || capacity > Garage<IVehicle>.MaximumGarageCapacity)
        {
            ShowError($"Capacity must be a whole number between 1 and {Garage<IVehicle>.MaximumGarageCapacity}.");
            Capacity.SetFocus();
            return;
        }
        CreateRequested?.Invoke(name, capacity);
    }
}
