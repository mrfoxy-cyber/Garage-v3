using Garage.Terminal.Components;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class GaragesView : PageView
{
    public GaragePicker Picker { get; }
    public OperationFeedback Feedback { get; }

    public GaragesView(GarageTheme theme) : base("Garages", theme)
    {
        Picker = new(theme);
        Feedback = new(theme);

        AddPanel("Available garages", Picker);
        AddPanel("Feedback", Feedback);

        AddPanel("Cat garage", new AnimatedCatCar(theme));
    }
}
