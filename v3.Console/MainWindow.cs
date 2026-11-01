using Garage.Terminal.Components;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Views;

public sealed class MainWindow : Runnable
{
    public event Action<GarageMenuAction>? MenuRequested;

    public MainWindow()
    {
        Title = "Garage V3";

        GarageTheme theme = new();

        GarageView garageView = new(theme)
        {
            Height = Dim.Fill(1)
        };

        BottomCommandBar commands = new(theme);

        // Sample data for reviewing the layout.
        garageView.Summary.Update(new("City Garage", 20, 4));

        commands.OptionRequested += action =>
        {
            if (action == GarageMenuAction.Quit)
                App?.RequestStop();
            else
                MenuRequested?.Invoke(action);
        };

        Add(garageView, commands);
    }
}
