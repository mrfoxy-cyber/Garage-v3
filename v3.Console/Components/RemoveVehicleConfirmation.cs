using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class RemoveVehicleConfirmation : FormComponent
{
    private readonly Label summary;
    private readonly Button confirm;

    public LocatedVehicleInfo? Match { get; private set; }
    public event Action<LocatedVehicleInfo>? ConfirmRequested;
    public event Action? CancelRequested;

    public RemoveVehicleConfirmation(GarageTheme theme) : base(theme)
    {
        summary = AddTextBlock("Find a vehicle before confirming removal.");
        AddAction("_Cancel", () => CancelRequested?.Invoke());
        confirm = AddAction("_Remove this vehicle", () =>
        {
            if (Match is { } match)
                ConfirmRequested?.Invoke(match);
        });
        confirm.Enabled = false;
    }

    public void ShowMatch(LocatedVehicleInfo match)
    {
        ArgumentNullException.ThrowIfNull(match);
        Match = match;
        summary.Text = $"{match.Vehicle.RegistrationNumber} | {match.Vehicle.GetType().Name}\nParked at {match.GarageName}, spot {match.SpotNumber}.\nDo you wish to remove this vehicle?";
        confirm.Enabled = true;
        ShowError(null);
        RequestReflow();
    }

    public void Clear()
    {
        Match = null;
        confirm.Enabled = false;
        summary.Text = "Find a vehicle before confirming removal.";
        ShowError(null);
        RequestReflow();
    }
}
