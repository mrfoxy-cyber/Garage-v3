using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class GarageCard : FormComponent
{
    private readonly Label summary;
    private readonly Button park;
    private readonly Button remove;

    public GarageChoice Garage { get; private set; }
    public event Action<string>? OpenRequested;
    public event Action<string>? ParkRequested;
    public event Action<string>? RemoveRequested;

    public GarageCard(GarageChoice garage, GarageTheme theme) : base(theme)
    {
        Garage = garage;
        summary = AddTextBlock(string.Empty);
        AddAction("_Open garage", () => OpenRequested?.Invoke(Garage.Id));
        park = AddAction("_Park vehicle", () => ParkRequested?.Invoke(Garage.Id));
        remove = AddAction("_Remove vehicle", () => RemoveRequested?.Invoke(Garage.Id));
        Update(garage);
    }

    public void Update(GarageChoice garage)
    {
        ArgumentNullException.ThrowIfNull(garage);
        Garage = garage;
        summary.Text = $"{garage.Name}\n{garage.VehicleTypes}\n{garage.OccupiedSpaces}/{garage.Capacity} occupied | {garage.AvailableSpaces} available";
        park.Enabled = garage.AvailableSpaces > 0;
        remove.Enabled = garage.OccupiedSpaces > 0;
        RequestReflow();
    }
}
