using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class FindVehicleView : PageView
{
    public string? GarageId { get; }
    public FindVehicleInput Input { get; }
    public VehicleDirectory Results { get; }
    public OperationFeedback Feedback { get; }

    public FindVehicleView(GarageTheme theme, GarageChoice? garage = null) : base(garage is null ? "Find vehicle" : $"Find vehicle — {garage.Name}", theme)
    {
        GarageId = garage?.Id;
        Input = new(theme);
        Results = new(theme);
        Feedback = new(theme);

        AddPanel("Registration number", Input);
        AddPanel("Results", Results);
        AddPanel("Feedback", Feedback);
    }
}
