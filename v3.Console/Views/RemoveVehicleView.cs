using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class RemoveVehicleView : PageView
{
    public string? GarageId { get; }
    public RemoveVehicleInput Input { get; }
    public OperationFeedback Feedback { get; }

    public RemoveVehicleView(GarageTheme theme, GarageChoice? garage = null) : base(garage is null ? "Remove vehicle" : $"Remove vehicle — {garage.Name}", theme)
    {
        GarageId = garage?.Id;
        Input = new(theme);
        Feedback = new(theme);

        AddPanel("Find the vehicle to remove", Input);
        AddPanel("Feedback", Feedback);
    }
}
