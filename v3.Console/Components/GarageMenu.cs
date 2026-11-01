using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Terminal.Gui.Input;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public enum GarageMenuAction
{
    CreateGarage,
    ParkVehicle,
    RemoveVehicle,
    FindVehicle,
    Refresh,
    SearchVehicles,
    SelectGarage,
    ShowTypeCounts,
    Quit
}

public sealed class GarageMenu : View, IAdaptiveComponent
{
    private const int ControlPadding = 6;

    private readonly Shortcut toggle;
    private readonly List<(string Label, Shortcut Control)> options = [];

    public bool IsExpanded { get; private set; } = true;

    public event Action<GarageMenuAction>? OptionRequested;
    public event Action? SizeHintsChanged;

    public int MinimumWidth => options.Max(item => TextWidth(item.Control.Key.ToString())) + ControlPadding;

    public int PreferredWidth => IsExpanded
        ? Math.Max(TextWidth("Collapse F8") + ControlPadding, options.Max(item => TextWidth(item.Label) + TextWidth(item.Control.Key.ToString()) + ControlPadding))
        : MinimumWidth;

    public GarageMenu()
    {
        CanFocus = true;
        Height = Dim.Auto(DimAutoStyle.Content);

        toggle = new Shortcut(Key.F8, "Collapse", Toggle)
        {
            BindKeyToApplication = true,
            AlignmentModes = AlignmentModes.EndToStart,
            Width = Dim.Fill(),
            Height = 1
        };

        Add(toggle);

        AddOption(GarageMenuAction.CreateGarage, "New garage", Key.F1);
        AddOption(GarageMenuAction.ParkVehicle, "Park vehicle", Key.F2);
        AddOption(GarageMenuAction.RemoveVehicle, "Remove vehicle", Key.F3);
        AddOption(GarageMenuAction.FindVehicle, "Find vehicle", Key.F4);
        AddOption(GarageMenuAction.Refresh, "Refresh", Key.F5);
        AddOption(GarageMenuAction.SearchVehicles, "Search vehicles", Key.F6);
        AddOption(GarageMenuAction.SelectGarage, "Garages", Key.F7);
        AddOption(GarageMenuAction.ShowTypeCounts, "Vehicle types", Key.F9);
        AddOption(GarageMenuAction.Quit, "Quit", Key.F10);
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        // One row per option, plus the toggle and a separating row.
        return options.Count + 2;
    }

    public void Toggle()
    {
        SetExpanded(!IsExpanded);
    }

    public void SetExpanded(bool expanded)
    {
        if (IsExpanded == expanded)
            return;

        IsExpanded = expanded;
        toggle.Title = expanded ? "Collapse" : ">";

        foreach (var option in options)
            option.Control.CommandView.Visible = expanded;

        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }

    private void AddOption(GarageMenuAction action, string label, Key key)
    {
        View previous = options.Count == 0 ? toggle : options[^1].Control;

        Shortcut shortcut = new(key, label, () => OptionRequested?.Invoke(action))
        {
            BindKeyToApplication = true,
            AlignmentModes = AlignmentModes.EndToStart,
            X = 0,
            Y = Pos.Bottom(previous) + (options.Count == 0 ? 1 : 0),
            Width = Dim.Fill(),
            Height = 1
        };

        options.Add((label, shortcut));
        Add(shortcut);
    }

    private static int TextWidth(string text) => TextFormatter.GetWidestLineLength(text, 4);
}
