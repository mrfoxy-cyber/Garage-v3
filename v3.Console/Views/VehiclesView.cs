using Garage.Terminal.Components;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class VehiclesView : PageView
{
    public VehicleDirectory Vehicles { get; }
    public OperationFeedback Feedback { get; }

    public VehiclesView(GarageTheme theme) : base("All vehicles", theme)
    {
        Vehicles = new(theme);
        Feedback = new(theme);

        AddPanel("Parked vehicles", Vehicles);
        AddPanel("Feedback", Feedback);
    }
}
