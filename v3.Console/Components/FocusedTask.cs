using Garage.Terminal.Components.Shared;
using Garage.Terminal.Layout;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Garage.Terminal.Interfaces;

namespace Garage.Terminal.Components;

public sealed class FocusedTask<T> : AdaptiveComponent where T : View, IAdaptiveComponent
{
    private readonly Button back;
    private readonly Label title;

    public T Content { get; }
    public event Action? BackRequested;

    public override int MinimumWidth => Math.Max(Content.MinimumWidth, ComponentMetrics.NaturalWidth(back));
    public override int PreferredWidth => Math.Max(Content.PreferredWidth, ComponentMetrics.NaturalWidth(back) + 1 + ComponentMetrics.NaturalWidth(title));

    public FocusedTask(string heading, T content)
    {
        Content = content;
        CanFocus = true;
        back = new() { Text = "_Back", Height = Dim.Auto() };
        back.Accepted += (_, _) => BackRequested?.Invoke();
        title = new() { Text = heading };
        title.TextFormatter.WordWrap = true;
        Content.SizeHintsChanged += RequestReflow;
        Add(back, title, Content);
        SubViewLayout += (_, _) => Arrange();
    }

    public override int MeasureHeight(int availableWidth) => HeaderHeight(availableWidth) + 1 + Content.MeasureHeight(availableWidth);

    private int HeaderHeight(int width) => ComponentMetrics.MeasureHeight(back, width) + ComponentMetrics.MeasureHeight(title, width);

    private void Arrange()
    {
        int width = Math.Max(1, Viewport.Width);
        back.Width = Math.Min(width, ComponentMetrics.NaturalWidth(back));
        title.Y = ComponentMetrics.MeasureHeight(back, width);
        title.Width = width;
        title.Height = ComponentMetrics.MeasureHeight(title, width);
        Content.X = 0;
        Content.Y = HeaderHeight(width) + 1;
        Content.Width = width;
        Content.Height = Content.MeasureHeight(width);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Content.SizeHintsChanged -= RequestReflow;
        base.Dispose(disposing);
    }
}
