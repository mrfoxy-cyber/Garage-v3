using Garage.Terminal.Components;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class GlobalRemoveVehicleView : PageView
{
    public RemoveVehicleInput Input { get; }
    public OperationFeedback Feedback { get; }

    public GlobalRemoveVehicleView(GarageTheme theme) : base("Remove vehicle — all garages", theme)
    {
        Input = new(theme);
        Feedback = new(theme);

        AddPanel("Find a vehicle across all garages", Input);
        AddPanel("Feedback", Feedback);
    }
}
