using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Garage_V3.queries;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class VehicleListComponent : AdaptiveComponent
{
    private readonly GarageTheme theme;
    private readonly List<(ParkedVehicleInfo Entry, CollapsiblePanel<VehicleSpecifications> Panel)> rows = [];
    private readonly Label empty;

    public ParkedVehicleInfo? SelectedVehicle { get; private set; }
    public event Action<ParkedVehicleInfo?>? SelectionChanged;

    public override int MinimumWidth => rows.Select(row => row.Panel.MinimumWidth).DefaultIfEmpty(20).Max();
    public override int PreferredWidth => rows.Select(row => row.Panel.PreferredWidth).DefaultIfEmpty(40).Max();

    public VehicleListComponent(GarageTheme theme)
    {
        this.theme = theme;
        CanFocus = true;
        SchemeName = theme.SurfaceScheme;
        empty = new() { Text = "No vehicles are parked.", Width = Dim.Fill(), Height = Dim.Auto(DimAutoStyle.Text) };
        empty.TextFormatter.WordWrap = true;
        Add(empty);
        SubViewLayout += (_, _) => ArrangeRows();
    }

    public void ShowVehicles(IReadOnlyList<ParkedVehicleInfo> entries, string emptyMessage = "No vehicles are parked.")
    {
        ArgumentNullException.ThrowIfNull(entries);
        foreach (var row in rows)
        {
            Remove(row.Panel);
            row.Panel.Dispose();
        }
        rows.Clear();
        SelectedVehicle = null;
        empty.Text = emptyMessage;
        empty.Visible = entries.Count == 0;

        foreach (var entry in entries.OrderBy(entry => entry.SpotNumber))
        {
            VehicleSpecifications details = new(theme);
            details.ShowVehicle(entry.Vehicle);
            string title = $"Spot {entry.SpotNumber} | {entry.Vehicle.RegistrationNumber} | {entry.Vehicle.GetType().Name}";
            CollapsiblePanel<VehicleSpecifications> panel = new(title, details, theme, expanded: false);
            panel.SizeHintsChanged += RequestReflow;
            panel.HasFocusChanged += (_, _) => { if (panel.HasFocus) Select(entry); };
            rows.Add((entry, panel));
            Add(panel);
        }

        if (rows.Count > 0)
        {
            Select(rows[0].Entry);
            rows[0].Panel.SetFocus();
        }
        else
            SelectionChanged?.Invoke(null);

        RequestReflow();
    }

    public bool SelectRegistration(string registrationNumber)
    {
        var index = rows.FindIndex(row => string.Equals(row.Entry.Vehicle.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase));
        if (index < 0)
            return false;
        Select(rows[index].Entry);
        rows[index].Panel.SetFocus();
        return true;
    }

    public override int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);
        return rows.Count == 0 ? ComponentMetrics.MeasureHeight(empty, availableWidth) : rows.Sum(row => row.Panel.MeasureHeight(availableWidth)) + rows.Count - 1;
    }

    private void ArrangeRows()
    {
        int width = Math.Max(1, Viewport.Width);
        int top = 0;
        foreach (var row in rows)
        {
            row.Panel.X = 0;
            row.Panel.Y = top;
            row.Panel.Width = width;
            row.Panel.Height = row.Panel.MeasureHeight(width);
            top += row.Panel.MeasureHeight(width) + 1;
        }
    }

    private void Select(ParkedVehicleInfo entry)
    {
        SelectedVehicle = entry;
        SelectionChanged?.Invoke(entry);
    }
}
