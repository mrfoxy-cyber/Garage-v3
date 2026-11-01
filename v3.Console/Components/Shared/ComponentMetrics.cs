using System.Drawing;
using Garage.Terminal.Layout;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Garage.Terminal.Interfaces;

namespace Garage.Terminal.Components.Shared;

public static class ComponentMetrics
{
    public static int TextWidth(string text) => TextFormatter.GetWidestLineLength(text, 4);

    public static int NaturalWidth(View control)
    {
        if (control is IAdaptiveComponent adaptive)
            return adaptive.PreferredWidth;

        return Math.Max(1, TextWidth(control.TextFormatter.Text)) + control.GetAdornmentsThickness().Horizontal;
    }

    public static int MeasureHeight(View control, int width)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);

        if (control is IAdaptiveComponent adaptive)
            return adaptive.MeasureHeight(width);

        var adornments = control.GetAdornmentsThickness();
        if (control is TextField)
            return 1 + adornments.Vertical;

        TextFormatter formatter = new()
        {
            Text = control.TextFormatter.Text,
            WordWrap = control is Label,
            MultiLine = true,
            HotKeySpecifier = control.TextFormatter.HotKeySpecifier
        };

        var size = formatter.FormatAndGetSize(new Size(Math.Max(1, width - adornments.Horizontal), int.MaxValue));
        return Math.Max(1, size.Height) + adornments.Vertical;
    }
}
