using Garage.Terminal.Components;
using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Views;

public abstract class PageView : View
{
    private readonly AdaptiveLayout layout;
    private readonly GarageTheme theme;

    public event Action? BackRequested;

    protected PageView(string heading, GarageTheme theme, bool showBack = true)
    {
        this.theme = theme;

        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();
        SchemeName = theme.SurfaceScheme;

        View header = new()
        {
            CanFocus = showBack,
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Content),
            SchemeName = theme.PanelHeaderScheme
        };

        Button back = new()
        {
            Text = "_Back",
            Visible = showBack,
            Width = Dim.Auto(),
            Height = Dim.Auto()
        };

        theme.StyleChoice(back);
        back.SchemeName = theme.PanelHeaderScheme;
        back.Accepted += (_, _) => BackRequested?.Invoke();

        Label title = new()
        {
            Text = heading,
            X = showBack ? Pos.Right(back) + 1 : 1,
            Width = Dim.Fill(1),
            Height = Dim.Auto(DimAutoStyle.Text),
            SchemeName = theme.PanelHeaderScheme
        };

        title.TextFormatter.WordWrap = true;

        header.Add(back, title);

        layout = new()
        {
            X = 1,
            Y = Pos.Bottom(header) + 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill()
        };

        Add(header, layout);
    }

    protected void AddPanel<T>(string title, T component, bool expanded = true) where T : View, IAdaptiveComponent
    {
        layout.AddComponent(new CollapsiblePanel<T>(title, component, theme, expanded));
    }
}
