using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class SearchVehiclesView : PageView
{
    public string? GarageId { get; }
    public VehicleSearchInput Input { get; }
    public VehicleDirectory Results { get; }
    public OperationFeedback Feedback { get; }

    public SearchVehiclesView(GarageTheme theme, GarageChoice? garage = null) : base(garage is null ? "Search vehicles" : $"Search vehicles — {garage.Name}", theme)
    {
        GarageId = garage?.Id;
        Input = new(theme);
        Results = new(theme);
        Feedback = new(theme);

        AddPanel("Search filters", Input);
        AddPanel("Matching vehicles", Results);
        AddPanel("Feedback", Feedback);
    }
}
