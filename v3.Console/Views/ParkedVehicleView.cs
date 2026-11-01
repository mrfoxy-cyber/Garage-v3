using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class ParkVehicleView : PageView
{
    public string GarageId { get; }
    public ParkVehicleInput Input { get; }
    public OperationFeedback Feedback { get; }

    public ParkVehicleView(GarageTheme theme, GarageChoice garage) : base($"Park vehicle — {garage.Name}", theme)
    {
        GarageId = garage.Id;
        Input = new(theme);
        Feedback = new(theme);

        AddPanel("Vehicle information", Input);
        AddPanel("Feedback", Feedback);
    }
}
