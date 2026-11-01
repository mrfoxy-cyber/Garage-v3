using System.Collections.ObjectModel;
using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class GaragePicker : AdaptiveComponent
{
    private IReadOnlyList<GarageChoice> garages = Array.Empty<GarageChoice>();
    private readonly Label caption;
    private readonly ListView list;
    private readonly Button create;
    private readonly Button open;

    public GarageChoice? SelectedGarage => list.Value is int index && index >= 0 && index < garages.Count ? garages[index] : null;
    public event Action<string>? OpenRequested;
    public event Action? CreateRequested;
    public event Action<GarageChoice?>? SelectionChanged;

    public override int MinimumWidth => Math.Max(ComponentMetrics.NaturalWidth(create), ComponentMetrics.NaturalWidth(open));
    public override int PreferredWidth => Math.Max(50, MinimumWidth);

    private int ListRows => Math.Min(8, garages.Count);

    public GaragePicker(GarageTheme theme)
    {
        CanFocus = true;
        SchemeName = theme.SurfaceScheme;
        caption = new() { Text = "No garages yet. Create one to get started." };
        caption.TextFormatter.WordWrap = true;
        list = new() { Visible = false };
        create = new() { Text = "_Create garage", Height = Dim.Auto() };
        open = new() { Text = "_Open garage", Height = Dim.Auto(), Visible = false };

        create.Accepted += (_, _) => CreateRequested?.Invoke();
        open.Accepted += (_, _) => { if (SelectedGarage is { } garage) OpenRequested?.Invoke(garage.Id); };
        list.ValueChanged += (_, _) =>
        {
            open.Enabled = SelectedGarage is not null;
            SelectionChanged?.Invoke(SelectedGarage);
        };

        Add(caption, list, create, open);
        SubViewLayout += (_, _) => Arrange();
    }

    public void ShowGarages(IReadOnlyList<GarageChoice> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        string? previous = SelectedGarage?.Id;
        garages = items.ToArray();
        list.Source = new ListWrapper<string>(new ObservableCollection<string>(garages.Select(garage => $"{garage.Name} | {garage.VehicleTypes} | {garage.OccupiedSpaces}/{garage.Capacity} occupied")));
        list.Visible = open.Visible = garages.Count > 0;
        caption.Text = garages.Count == 0 ? "No garages yet. Create one to get started." : "Choose a garage";
        list.Value = garages.Count == 0 ? null : Math.Max(0, garages.ToList().FindIndex(garage => garage.Id == previous));
        open.Enabled = SelectedGarage is not null;
        RequestReflow();
    }

    public override int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);
        int height = ComponentMetrics.MeasureHeight(caption, availableWidth) + 1;
        if (garages.Count > 0)
            height += ListRows + 1;
        int createHeight = ComponentMetrics.MeasureHeight(create, availableWidth);
        int openHeight = ComponentMetrics.MeasureHeight(open, availableWidth);
        bool sideBySide = ComponentMetrics.NaturalWidth(create) + ComponentMetrics.NaturalWidth(open) + 1 <= availableWidth;
        return height + (open.Visible ? sideBySide ? Math.Max(createHeight, openHeight) : createHeight + 1 + openHeight : createHeight);
    }

    private void Arrange()
    {
        int width = Math.Max(1, Viewport.Width);
        caption.Width = width;
        caption.Height = ComponentMetrics.MeasureHeight(caption, width);
        int top = ComponentMetrics.MeasureHeight(caption, width) + 1;
        list.Y = top;
        list.Width = width;
        list.Height = Math.Max(1, ListRows);
        if (garages.Count > 0)
            top += ListRows + 1;

        create.Y = top;
        create.Width = Math.Min(width, ComponentMetrics.NaturalWidth(create));
        bool sideBySide = ComponentMetrics.NaturalWidth(create) + ComponentMetrics.NaturalWidth(open) + 1 <= width;
        open.X = sideBySide ? ComponentMetrics.NaturalWidth(create) + 1 : 0;
        open.Y = sideBySide ? top : top + ComponentMetrics.MeasureHeight(create, width) + 1;
        open.Width = Math.Min(width, ComponentMetrics.NaturalWidth(open));
    }
}
