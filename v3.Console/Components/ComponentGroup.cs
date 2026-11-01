using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class ComponentGroup : View, IAdaptiveComponent
{
    private const int Padding = 1;
    private const int HeadingHeight = 1;
    private const int HeadingGap = 1;

    private readonly Label heading;
    private readonly AdaptiveLayout layout;

    public event Action? SizeHintsChanged;

    public int MinimumWidth => Math.Max(HeadingWidth, layout.MinimumWidth) + Padding * 2;
    public int PreferredWidth => Math.Max(HeadingWidth, layout.PreferredWidth) + Padding * 2;

    private int HeadingWidth => TextFormatter.GetWidestLineLength(heading.Text, 4);

    public ComponentGroup(string title, GarageTheme theme)
    {
        CanFocus = true;
        SchemeName = theme.GroupScheme;

        heading = new()
        {
            Text = title,
            X = Padding,
            Y = Padding,
            Width = Dim.Fill(Padding),
            Height = HeadingHeight
        };

        heading.TextFormatter.WordWrap = false;

        layout = new AdaptiveLayout(scrollable: false)
        {
            X = Padding,
            Y = Pos.Bottom(heading) + HeadingGap,
            Width = Dim.Fill(Padding),
            Height = Dim.Func(_ => layoutHeight())
        };

        layout.SizeHintsChanged += NotifyLayoutChanged;

        Height = Dim.Func(_ => MeasureHeight(Math.Max(1, Viewport.Width)));

        Add(heading, layout);
    }

    public void AddComponent<T>(T component) where T : View, IAdaptiveComponent
    {
        layout.AddComponent(component);
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        int innerWidth = Math.Max(1, availableWidth - Padding * 2);

        return Padding + HeadingHeight + HeadingGap + layout.MeasureHeight(innerWidth) + Padding;
    }

    private int layoutHeight()
    {
        return layout.MeasureHeight(Math.Max(1, Viewport.Width - Padding * 2));
    }

    private void NotifyLayoutChanged()
    {
        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }
}
