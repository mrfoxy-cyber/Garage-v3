using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;


namespace Garage.Terminal.Components.Shared;

public class AdaptiveText : AdaptiveComponent
{
    private readonly Label content;

    public string DisplayText => content.Text;
    public override int MinimumWidth => 12;
    public override int PreferredWidth => Math.Max(MinimumWidth, Math.Min(72, ComponentMetrics.TextWidth(content.Text)));

    public AdaptiveText(GarageTheme theme)
    {
        SchemeName = theme.SurfaceScheme;
        content = new() { Width = Dim.Fill(), Height = Dim.Auto(DimAutoStyle.Text) };
        content.TextFormatter.WordWrap = true;
        Add(content);
    }

    public void ShowText(string text)
    {
        content.Text = text;
        RequestReflow();
    }

    public override int MeasureHeight(int availableWidth) => ComponentMetrics.MeasureHeight(content, availableWidth);
}
