using Garage.Terminal.Components;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class GlobalSearchVehiclesView : PageView
{
    public VehicleSearchInput Input { get; }
    public VehicleDirectory Results { get; }
    public OperationFeedback Feedback { get; }

    public GlobalSearchVehiclesView(GarageTheme theme) : base("Search vehicles — all garages", theme)
    {
        Input = new(theme);
        Results = new(theme);
        Feedback = new(theme);

        AddPanel("Search filters", Input);
        AddPanel("Matches by garage", Results);
        AddPanel("Feedback", Feedback);
    }
}
