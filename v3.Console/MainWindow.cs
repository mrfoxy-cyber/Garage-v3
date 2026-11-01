using Garage.Terminal.Components;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Views;

public sealed class MainWindow : Runnable
{
    private readonly ScreenHost screenHost;

    public GarageTheme Theme { get; }
    public HomeView Home { get; }

    public event Action<GarageMenuAction>? MenuRequested;

    public MainWindow()
    {
        Title = "Garage V3";

        Theme = new();
        Theme.Apply();

        SchemeName = Theme.SurfaceScheme;

        BottomCommandBar commands = new(Theme)
        {
            Y = Pos.AnchorEnd(),
            Width = Dim.Fill()
        };

        screenHost = new()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(commands)
        };

        Home = new(Theme, []);
        screenHost.ShowPage(Home);

        commands.OptionRequested += action =>
        {
            if (action == GarageMenuAction.Quit)
            {
                App?.RequestStop();
                return;
            }

            MenuRequested?.Invoke(action);
        };

        Add(screenHost, commands);
    }

    public void ShowPage(PageView page)
    {
        screenHost.ShowPage(page);
    }
}
