using Garage.Terminal.Presentation;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class AppHeader : View
{
    private readonly Label content;

    public AppHeader(GarageTheme theme)
    {
        Width = Dim.Fill();
        Height = Dim.Auto(DimAutoStyle.Content);
        SchemeName = theme.PanelHeaderScheme;

        content = new()
        {
            Text = "Garage V3",
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Text)
        };

        content.TextFormatter.WordWrap = true;

        Add(content);
    }

    public void Show(HeaderContent header)
    {
        content.Text = string.IsNullOrWhiteSpace(header.Subtitle)
            ? header.Title
            : $"{header.Title}\n{header.Subtitle}";

        SetNeedsLayout();
    }
}
