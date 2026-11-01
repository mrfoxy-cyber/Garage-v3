using Garage.Terminal.Interfaces;
using System.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;

namespace Garage.Terminal.Layout;

public sealed class AdaptiveLayout : View
{
    private const int Gap = 2;

    private readonly List<(View Control, IAdaptiveComponent Sizing)> components = [];

    public AdaptiveLayout()
    {
        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();

        ViewportSettings |= ViewportSettingsFlags.HasScrollBars;

        SubViewLayout += (_, _) => ArrangeComponents();

        AddCommand(Command.ScrollDown, () => { ScrollVertical(5); return true; });
        AddCommand(Command.ScrollUp, () => { ScrollVertical(-5); return true; });

        KeyBindings.Add(Key.PageDown, Command.ScrollDown);
        KeyBindings.Add(Key.PageUp, Command.ScrollUp);
    }

    public void AddComponent<T>(T component) where T : View, IAdaptiveComponent
    {
        components.Add((component, component));
        component.SizeHintsChanged += () => SetNeedsLayout();

        Add(component);
        SetNeedsLayout();
    }

    private void ArrangeComponents()
    {
        if (components.Count == 0 || Viewport.Width <= 0)
            return;

        int availableWidth = Viewport.Width;

        int requiredWidth = components.Sum(item => item.Sizing.PreferredWidth)
            + Gap * (components.Count - 1);

        if (availableWidth >= requiredWidth)
        {
            ArrangeHorizontally(availableWidth);
        }
        else
        {
            ArrangeVertically(availableWidth);
        }
    }

    private void ArrangeHorizontally(int availableWidth)
    {
        int left = 0;
        int requiredHeight = 0;

        for (int index = 0; index < components.Count; index++)
        {
            var item = components[index];
            int remainingCount = components.Count - index - 1;

            int reservedWidth = components
                .Skip(index + 1)
                .Sum(next => next.Sizing.MinimumWidth);

            reservedWidth += Gap * remainingCount;

            int maximumWidth = availableWidth - left - reservedWidth;

            int width = remainingCount == 0
                ? maximumWidth
                : Math.Clamp(item.Sizing.PreferredWidth, item.Sizing.MinimumWidth, maximumWidth);

            int height = item.Sizing.MeasureHeight(width);

            Place(item.Control, left, 0, width, height);

            left += width + Gap;
            requiredHeight = Math.Max(requiredHeight, height);
        }

        UpdateContentSize(availableWidth, requiredHeight);
    }

    private void ArrangeVertically(int availableWidth)
    {
        int width = Math.Max(availableWidth, components.Max(item => item.Sizing.MinimumWidth));
        int top = 0;

        foreach (var item in components)
        {
            int height = item.Sizing.MeasureHeight(width);

            Place(item.Control, 0, top, width, height);

            top += height + Gap;
        }

        UpdateContentSize(width, top - Gap);
    }

    private static void Place(View control, int left, int top, int width, int height)
    {
        control.X = left;
        control.Y = top;
        control.Width = width;
        control.Height = height;
    }

    private void UpdateContentSize(int width, int height)
    {
        Size size = new(width, height);

        if (GetContentSize() != size)
            SetContentSize(size);
    }
}
