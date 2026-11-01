using Garage.Terminal.Interfaces;
using System.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;

namespace Garage.Terminal.Layout;

public sealed class AdaptiveLayout : View, IAdaptiveComponent
{
    private const int Gap = 2;

    private sealed record Item(View Control, IAdaptiveComponent Sizing);
    private sealed record Placement(View Control, Rectangle Bounds);

    private readonly List<Item> components = [];
    private readonly bool scrollable;

    public event Action? SizeHintsChanged;

    public int MinimumWidth => components.Where(item => item.Control.Visible)
        .Select(item => item.Sizing.MinimumWidth).DefaultIfEmpty(1).Max();

    public int PreferredWidth
    {
        get
        {
            var visible = components.Where(item => item.Control.Visible).ToList();
            return visible.Count == 0 ? 1 : visible.Sum(item => item.Sizing.PreferredWidth) + Gap * (visible.Count - 1);
        }
    }

    public AdaptiveLayout(bool scrollable = true)
    {
        this.scrollable = scrollable;

        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();

        if (scrollable)
        {
            ViewportSettings |= ViewportSettingsFlags.HasScrollBars;

            AddCommand(Command.ScrollDown, () => { ScrollVertical(5); return true; });
            AddCommand(Command.ScrollUp, () => { ScrollVertical(-5); return true; });

            KeyBindings.Add(Key.PageDown, Command.ScrollDown);
            KeyBindings.Add(Key.PageUp, Command.ScrollUp);
        }

        SubViewLayout += (_, _) => ArrangeComponents();
    }

    public void AddComponent<T>(T component) where T : View, IAdaptiveComponent
    {
        ArgumentNullException.ThrowIfNull(component);

        if (component.SuperView is not null)
            throw new InvalidOperationException("This component already belongs to another view. Create a new instance.");

        components.Add(new(component, component));
        component.SizeHintsChanged += NotifyLayoutChanged;

        Add(component);
        NotifyLayoutChanged();
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        return BuildLayout(availableWidth)
            .Select(item => item.Bounds.Bottom)
            .DefaultIfEmpty(0)
            .Max();
    }

    private List<Placement> BuildLayout(int availableWidth)
    {
        List<Placement> placements = [];

        int left = 0;
        int top = 0;
        int rowHeight = 0;

        foreach (var item in components.Where(item => item.Control.Visible))
        {
            int minimum = Math.Max(1, item.Sizing.MinimumWidth);
            int preferred = Math.Max(minimum, item.Sizing.PreferredWidth);
            int width = Math.Max(minimum, Math.Min(preferred, availableWidth));

            if (left > 0 && left + width > availableWidth)
            {
                left = 0;
                top += rowHeight + Gap;
                rowHeight = 0;
            }

            int height = Math.Max(1, item.Sizing.MeasureHeight(width));

            placements.Add(new(item.Control, new Rectangle(left, top, width, height)));

            left += width + Gap;
            rowHeight = Math.Max(rowHeight, height);
        }

        return placements;
    }

    private void ArrangeComponents()
    {
        if (Viewport.Width <= 0)
            return;

        var placements = BuildLayout(Viewport.Width);

        foreach (var item in placements)
        {
            item.Control.X = item.Bounds.X;
            item.Control.Y = item.Bounds.Y;
            item.Control.Width = item.Bounds.Width;
            item.Control.Height = item.Bounds.Height;
        }

        if (!scrollable)
            return;

        int width = Math.Max(Viewport.Width, placements.Select(item => item.Bounds.Right).DefaultIfEmpty(0).Max());
        int height = placements.Select(item => item.Bounds.Bottom).DefaultIfEmpty(0).Max();

        Size size = new(width, height);

        if (GetContentSize() != size)
            SetContentSize(size);
    }

    private void NotifyLayoutChanged()
    {
        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }
}
