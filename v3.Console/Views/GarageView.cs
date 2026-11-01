using Garage.Terminal.Components;
using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;

namespace Garage.Terminal.Views;

public sealed class GarageView : View
{
    public GarageInput Input { get; } = new();
    public GarageSummary Summary { get; } = new();

    public GarageView(GarageTheme theme)
    {
        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();

        AdaptiveLayout layout = new()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        for (int index = 1; index <= 4; index++)
        {
            GarageSummary summary = new();
            summary.Update(new($"Test garage {index}", 20, index));

            GarageInput input = new();

            layout.AddComponent(new CollapsiblePanel<GarageSummary>($"Garage information {index}", summary, theme));
            layout.AddComponent(new CollapsiblePanel<GarageInput>($"Create garage {index}", input, theme));
        }

        Add(layout);
    }
}
