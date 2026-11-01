using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class ConfirmRemovalView : PageView
{
    public RemoveVehicleConfirmation Confirmation { get; }
    public VehicleSpecifications Specifications { get; }
    public OperationFeedback Feedback { get; }

    public ConfirmRemovalView(GarageTheme theme, LocatedVehicleInfo match) : base("Confirm removal", theme)
    {
        Confirmation = new(theme);
        Specifications = new(theme);
        Feedback = new(theme);

        Confirmation.ShowMatch(match);
        Specifications.ShowVehicle(match.Vehicle);

        AddPanel("Remove this vehicle?", Confirmation);
        AddPanel("Vehicle specifications", Specifications, expanded: false);
        AddPanel("Feedback", Feedback);
    }
}
