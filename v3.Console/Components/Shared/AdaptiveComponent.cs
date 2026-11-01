using Garage.Terminal.Layout;
using Terminal.Gui.ViewBase;
using Garage.Terminal.Interfaces;

namespace Garage.Terminal.Components.Shared;

public abstract class AdaptiveComponent : View, IAdaptiveComponent
{
    public abstract int MinimumWidth { get; }
    public abstract int PreferredWidth { get; }
    public abstract int MeasureHeight(int availableWidth);

    public event Action? SizeHintsChanged;

    protected AdaptiveComponent()
    {
        Height = Dim.Func(_ => MeasureHeight(Math.Max(1, Viewport.Width)));
    }

    protected void RequestReflow()
    {
        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }
}
