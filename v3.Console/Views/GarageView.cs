using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class GarageView : PageView
{
    public GarageCard Summary { get; }
    public VehicleListComponent Vehicles { get; }
    public VehicleTypeCounts TypeCounts { get; }
    public OperationFeedback Feedback { get; }

    public GarageView(GarageTheme theme, GarageChoice garage) : base(garage.Name, theme)
    {
        Summary = new(garage, theme);
        Vehicles = new(theme);
        TypeCounts = new(theme);
        Feedback = new(theme);

        AddPanel("Garage information", Summary);
        AddPanel("Parked vehicles", Vehicles);
        AddPanel("Vehicle types", TypeCounts);
        AddPanel("Feedback", Feedback);
    }
}
