using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Garage_V3.queries;
using System.Drawing;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class GarageSummary : View, IAdaptiveComponent
{
    private readonly Label content;
    public event Action? SizeHintsChanged;

    public int MinimumWidth => TextFormatter.GetWidestLineLength("Available:", 4) + 2;
    public int PreferredWidth => Math.Max(MinimumWidth, TextFormatter.GetWidestLineLength(content.Text, 4));

    public GarageSummary()
    {
        content = new()
        {
            Text = "No garage selected.",
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Text)
        };

        content.TextFormatter.WordWrap = true;

        Add(content);

        Height = Dim.Func(_ => MeasureHeight(Math.Max(1, Viewport.Width)));
    }

    public void Update(GarageInfo? garage)
    {
        content.Text = garage is null
            ? "No garage selected."
            : $"{garage.GarageName}\nCapacity: {garage.Capacity} | Occupied: {garage.OccupiedSpaces} | Available: {garage.AvailableSpaces}";

        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        TextFormatter formatter = new()
        {
            Text = content.Text,
            WordWrap = true,
            MultiLine = true
        };

        var size = formatter.FormatAndGetSize(new Size(availableWidth, int.MaxValue));

        return Math.Max(1, size.Height);
    }
}
