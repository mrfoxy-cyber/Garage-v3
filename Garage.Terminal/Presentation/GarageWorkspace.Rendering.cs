using System.Data;
using Garage.Vehicles;
using Terminal.Gui.Views;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Terminal.Presentation;

internal sealed partial class GarageWorkspace
{
    private IReadOnlyList<Vehicle> displayedVehicles = [];
    private IReadOnlyList<VehicleGarage> displayedGarages = [];
    private bool refreshing;

    private Vehicle? SelectedVehicle => layout.Table.Value?.SelectedCell.Y is int row && row >= 0 && row < displayedVehicles.Count
        ? displayedVehicles[row] : null;

    private void RefreshRows()
    {
        if (refreshing) return;
        refreshing = true;
        try
        {
            int selected = layout.Table.Value?.SelectedCell.Y ?? 0;
            buttons.GaragePicker.Text = state.Garage is null ? "Choose a garage" : $"Garage: {state.Garage.GarageName}  ▾";
            layout.Stats.Text = $"{state.Service.Garages.Count} GARAGES   {state.Service.Catalog.Vehicles.Count} VEHICLES   " +
                $"{state.Service.Garages.Sum(g => g.OccupiedSpaces)} PARKED\n" +
                (state.Garage is null ? "Create your first garage to start parking." :
                    $"{state.Garage.AvailableSpaces} spots available in {state.Garage.GarageName}");
            DataTable data = new();
            if (state.Page == WorkspacePage.Overview)
            {
                displayedVehicles = [];
                AddColumns(data, ["Garage", "Parked", "Available", "Capacity", "Occupancy"]);
                displayedGarages = state.Service.Garages.Where(garage => garage.GarageName.Contains(
                    state.QuickFilter.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (VehicleGarage garage in displayedGarages)
                {
                    int filled = (int)Math.Round(12d * garage.OccupiedSpaces / garage.Capacity);
                    data.Rows.Add(garage.GarageName, garage.OccupiedSpaces, garage.AvailableSpaces, garage.Capacity,
                        new string('█', filled) + new string('░', 12 - filled));
                }
                layout.TableFrame.Title = "Garage overview / Enter to open";
            }
            else
            {
                displayedVehicles = state.VisibleVehicles();
                AddColumns(data, ["Registration", "Type", "Vehicle", "Year", "Location", "Spot"]);
                foreach (Vehicle vehicle in displayedVehicles)
                    data.Rows.Add(vehicle.RegistrationNumber, vehicle.GetType().Name, $"{vehicle.Manufacturer} {vehicle.Model}",
                        vehicle.ManufacturingYear, state.Location(vehicle)?.GarageName ?? "Available", state.Spot(vehicle)?.ToString() ?? "—");
                layout.TableFrame.Title = $"{state.Page} / {displayedVehicles.Count} vehicle(s)";
            }
            layout.Table.Table = new DataTableSource(data);
            if (data.Rows.Count > 0) layout.Table.SetSelection(0, Math.Clamp(selected, 0, data.Rows.Count - 1), false);
            buttons.Park.Enabled = state.Garage is not null;
            buttons.Remove.Enabled = state.Page is not WorkspacePage.Overview;
            buttons.Spots.Enabled = state.Garage is not null;
            UpdateDetails();
        }
        finally { refreshing = false; }
    }

    private void UpdateDetails()
    {
        if (state.Page == WorkspacePage.Overview)
            layout.Details.Text = "Your garages, capacity, and availability in one place.\nEnter opens a garage. F4 creates a new layout.\nTab moves focus. All changes stay in this session.";
        else if (SelectedVehicle is Vehicle vehicle) layout.Details.Text = VehicleDetails.Summary(vehicle);
        else layout.Details.Text = "No vehicles match this view.\nClear the Find field or adjust your filters.\nF2 creates a vehicle; Import vehicle opens a JSON file.";
    }

    private static void AddColumns(DataTable data, List<string> names)
    {
        foreach (string name in names)
            data.Columns.Add(name);
    }
}
