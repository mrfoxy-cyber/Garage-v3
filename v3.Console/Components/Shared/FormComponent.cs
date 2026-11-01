using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Garage.Terminal.Interfaces;

namespace Garage.Terminal.Components.Shared;

public abstract class FormComponent : AdaptiveComponent
{
    private sealed record Item(View Control, int Gap, bool Stretch);
    private readonly List<Item> items = [];
    private readonly Label error;

    public string ErrorMessage => error.Text;
    public override int MinimumWidth => Math.Max(12, items.Select(item => MinimumFor(item.Control)).DefaultIfEmpty(1).Max());
    public override int PreferredWidth => Math.Max(30, items.Select(item => ComponentMetrics.NaturalWidth(item.Control)).DefaultIfEmpty(1).Max());

    protected FormComponent(GarageTheme theme)
    {
        CanFocus = true;
        SchemeName = theme.SurfaceScheme;

        error = new() { Visible = false, SchemeName = theme.ErrorScheme };
        error.TextFormatter.WordWrap = true;
        Add(error);

        SubViewLayout += (_, _) => Arrange();
    }

    protected TextField AddTextInput(string caption, string initialValue = "")
    {
        AddCaption(caption);
        TextField field = new() { Text = initialValue };
        AddItem(field, 0, true);
        return field;
    }

    protected ChoiceInput AddChoiceInput(string caption, IEnumerable<string> choices, string? selected = null)
    {
        AddCaption(caption);
        ChoiceInput field = new(caption, choices, selected);
        field.SizeHintsChanged += RequestReflow;
        AddItem(field, 0, true);
        return field;
    }

    protected Button AddAction(string caption, Action action)
    {
        Button button = new() { Text = caption, Width = Dim.Auto(), Height = Dim.Auto() };
        button.TextFormatter.WordWrap = false;
        button.Accepted += (_, _) => action();
        AddItem(button, 1, false);
        return button;
    }

    protected Label AddTextBlock(string text)
    {
        Label label = new() { Text = text };
        label.TextFormatter.WordWrap = true;
        AddItem(label, items.Count == 0 ? 0 : 1, true);
        return label;
    }

    public void ShowError(string? message)
    {
        error.Text = message ?? string.Empty;
        error.Visible = !string.IsNullOrWhiteSpace(message);
        RequestReflow();
    }

    public override int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);
        int height = items.Sum(item => item.Gap + ComponentMetrics.MeasureHeight(item.Control, availableWidth));
        return error.Visible ? height + 1 + ComponentMetrics.MeasureHeight(error, availableWidth) : height;
    }

    private void AddCaption(string caption)
    {
        Label label = new() { Text = caption };
        label.TextFormatter.WordWrap = true;
        AddItem(label, items.Count == 0 ? 0 : 1, true);
    }

    private void AddItem(View control, int gap, bool stretch)
    {
        items.Add(new(control, gap, stretch));
        Add(control);
        RequestReflow();
    }

    private void Arrange()
    {
        int width = Math.Max(1, Viewport.Width);
        int top = 0;

        foreach (var item in items)
        {
            top += item.Gap;
            item.Control.X = 0;
            item.Control.Y = top;
            item.Control.Width = item.Stretch ? width : Math.Min(width, ComponentMetrics.NaturalWidth(item.Control));
            item.Control.Height = ComponentMetrics.MeasureHeight(item.Control, width);
            top += ComponentMetrics.MeasureHeight(item.Control, width);
        }

        if (error.Visible)
        {
            error.X = 0;
            error.Y = top + 1;
            error.Width = width;
            error.Height = ComponentMetrics.MeasureHeight(error, width);
        }
    }

    private static int MinimumFor(View control) => control is IAdaptiveComponent adaptive ? adaptive.MinimumWidth : control is TextField ? 12 : ComponentMetrics.NaturalWidth(control);
}
