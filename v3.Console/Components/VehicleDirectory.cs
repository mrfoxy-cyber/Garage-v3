using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Garage_V3.queries;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class VehicleDirectory : AdaptiveComponent
{
    private readonly GarageTheme theme;
    private readonly List<CollapsiblePanel<VehicleListComponent>> groups = [];
    private readonly Label empty;

    public LocatedVehicleInfo? SelectedVehicle { get; private set; }
    public event Action<LocatedVehicleInfo?>? SelectionChanged;

    public override int MinimumWidth => groups.Select(group => group.MinimumWidth).DefaultIfEmpty(20).Max();
    public override int PreferredWidth => groups.Select(group => group.PreferredWidth).DefaultIfEmpty(40).Max();

    public VehicleDirectory(GarageTheme theme)
    {
        this.theme = theme;
        CanFocus = true;
        SchemeName = theme.SurfaceScheme;
        empty = new() { Text = "No parked vehicles found.", Width = Dim.Fill(), Height = Dim.Auto(DimAutoStyle.Text) };
        empty.TextFormatter.WordWrap = true;
        Add(empty);
        SubViewLayout += (_, _) => Arrange();
    }

    public void ShowVehicles(IReadOnlyList<LocatedVehicleInfo> vehicles)
    {
        ArgumentNullException.ThrowIfNull(vehicles);
        foreach (var group in groups)
        {
            Remove(group);
            group.Dispose();
        }
        groups.Clear();
        SelectedVehicle = null;
        empty.Visible = vehicles.Count == 0;

        foreach (var garage in vehicles.GroupBy(vehicle => vehicle.GarageId))
        {
            var locations = garage.OrderBy(vehicle => vehicle.SpotNumber).ToArray();
            VehicleListComponent list = new(theme);
            list.ShowVehicles(locations.Select(location => new ParkedVehicleInfo(location.SpotNumber, location.Vehicle)).ToArray());
            list.SelectionChanged += selected =>
            {
                if (selected is null)
                    return;
                SelectedVehicle = locations.First(location => location.SpotNumber == selected.SpotNumber && ReferenceEquals(location.Vehicle, selected.Vehicle));
                SelectionChanged?.Invoke(SelectedVehicle);
            };
            CollapsiblePanel<VehicleListComponent> panel = new($"{locations[0].GarageName} | {locations.Length} vehicles", list, theme);
            panel.SizeHintsChanged += RequestReflow;
            groups.Add(panel);
            Add(panel);
        }

        SelectedVehicle = vehicles.FirstOrDefault();
        SelectionChanged?.Invoke(SelectedVehicle);
        RequestReflow();
    }

    public override int MeasureHeight(int availableWidth) => groups.Count == 0 ? ComponentMetrics.MeasureHeight(empty, availableWidth) : groups.Sum(group => group.MeasureHeight(availableWidth)) + groups.Count - 1;

    private void Arrange()
    {
        int width = Math.Max(1, Viewport.Width);
        int top = 0;
        foreach (var group in groups)
        {
            group.X = 0;
            group.Y = top;
            group.Width = width;
            group.Height = group.MeasureHeight(width);
            top += group.MeasureHeight(width) + 1;
        }
    }
}
