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
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Content)
        };

        Button back = new()
        {
            Text = "_Back",
            Visible = showBack,
            Width = Dim.Auto(),
            Height = Dim.Auto()
        };

        Label title = new()
        {
            Text = heading,
            X = showBack ? Pos.Right(back) + 1 : 0,
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Text)
        };

        title.TextFormatter.WordWrap = true;
        back.Accepted += (_, _) => BackRequested?.Invoke();

        header.Add(back, title);

        layout = new()
        {
            Y = Pos.Bottom(header),
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        Add(header, layout);
    }

    protected void AddPanel<T>(string title, T component, bool expanded = true) where T : View, IAdaptiveComponent
    {
        layout.AddComponent(new CollapsiblePanel<T>(title, component, theme, expanded));
    }
}
