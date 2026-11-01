using System.Collections.ObjectModel;
using Garage.Terminal.Components.Shared;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class ChoiceInput : AdaptiveComponent
{
    private readonly string caption;
    private readonly Button button;
    private IReadOnlyList<string> choices = Array.Empty<string>();

    public string Value { get; private set; } = string.Empty;
    public event Action<string>? ValueChanged;

    public override int MinimumWidth => Math.Max(12, ComponentMetrics.NaturalWidth(button));
    public override int PreferredWidth => Math.Max(MinimumWidth, choices.Select(ComponentMetrics.TextWidth).DefaultIfEmpty(1).Max() + 8);

    public ChoiceInput(string caption, IEnumerable<string> choices, string? selected = null)
    {
        this.caption = caption;
        CanFocus = true;
        button = new() { Width = Dim.Fill(), Height = Dim.Auto() };
        button.TextFormatter.WordWrap = false;
        button.Accepted += (_, _) => OpenPicker();
        Add(button);
        SetOptions(choices, selected);
    }

    public void SetOptions(IEnumerable<string> options, string? selected = null)
    {
        choices = options.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        button.Enabled = choices.Count > 0;
        string next = choices.FirstOrDefault(value => string.Equals(value, selected ?? Value, StringComparison.OrdinalIgnoreCase)) ?? choices.FirstOrDefault() ?? string.Empty;
        UpdateValue(next);
    }

    public void SetValue(string value)
    {
        string match = choices.FirstOrDefault(choice => string.Equals(choice, value, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"'{value}' is not an available choice.", nameof(value));
        UpdateValue(match);
    }

    public override int MeasureHeight(int availableWidth) => ComponentMetrics.MeasureHeight(button, availableWidth);

    private void UpdateValue(string value)
    {
        bool changed = Value != value;
        Value = value;
        button.Text = choices.Count == 0 ? "No choices" : $"{Value} v";
        RequestReflow();
        if (changed)
            ValueChanged?.Invoke(Value);
    }

    private void OpenPicker()
    {
        if (choices.Count == 0 || App is null)
            return;

        using Window popup = new()
        {
            Title = caption,
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Auto(DimAutoStyle.Content, minimumContentDim: PreferredWidth + 4, maximumContentDim: Dim.Percent(100)),
            Height = Math.Min(choices.Count + 5, 14)
        };

        ListView list = new()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(3),
            Source = new ListWrapper<string>(new ObservableCollection<string>(choices)),
            Value = Math.Max(0, choices.ToList().IndexOf(Value))
        };

        Button cancel = new() { Text = "Ca_ncel", Y = Pos.AnchorEnd() };
        Button choose = new() { Text = "_Choose", X = Pos.Right(cancel) + 1, Y = cancel.Y };
        string? selection = null;
        cancel.Accepted += (_, _) => App.RequestStop();
        choose.Accepted += (_, _) =>
        {
            if (list.Value is not int index || index < 0 || index >= choices.Count)
                return;
            selection = choices[index];
            App.RequestStop();
        };

        popup.Add(list, cancel, choose);
        App.Run(popup);
        if (selection is not null)
            SetValue(selection);
    }
}
