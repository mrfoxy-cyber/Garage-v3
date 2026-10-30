using Garage.Diagnostics;
using Garage.Results;
using Garage.Terminal.Forms;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Terminal.Presentation;

internal sealed partial class GarageWorkspace
{
    private void ChooseGarage()
    {
        int? selected = dialogs.Choose("Choose a garage", state.Service.Garages
            .Select(garage => $"{garage.GarageName} / {garage.OccupiedSpaces} parked / " +
                $"{garage.AvailableSpaces} available")
            .ToList());

        if (selected is int index)
        {
            state.Garage = state.Service.Garages[index];
            RefreshRows();
        }
    }

    private void OpenDetails()
    {
        if (state.Page == WorkspacePage.Overview)
        {
            int row = layout.Table.Value?.SelectedCell.Y ?? -1;
            if (row >= 0 && row < displayedGarages.Count)
            {
                state.Garage = displayedGarages[row];
                layout.Navigation.SelectedItem = 2;
                state.Page = WorkspacePage.Parking;
                layout.Query.Text = "";
                RefreshRows();
            }
        }
        else if (SelectedVehicle is Vehicle vehicle)
        {
            dialogs.Message("Vehicle details", VehicleDetails.Full(vehicle));
        }
    }

    private void CreateVehicle(bool parkImmediately)
    {
        if (parkImmediately && state.Garage is null)
        {
            ChooseGarage();
            if (state.Garage is null) return;
        }
        int? spot = parkImmediately ? ChooseSpot(null) : null;
        if (parkImmediately && spot is null) return;
        VehicleCreationResult? result = workflows.CreateVehicle(
            parkImmediately ? state.Garage : null,
            spot == 0 ? null : spot);
        if (result is not null)
        {
            SetStatus(result.Message);
            if (!result.Success)
                dialogs.Message("Vehicle creation", result.Message);
            else
                RevealVehicle(result.Vehicle!);
        }
        RefreshRows();
    }

    private void ImportVehicle()
    {
        string? path = dialogs.JsonFile("Open vehicle JSON / One vehicle object");
        if (path is null) return;
        int? mode = dialogs.Choose("Import vehicle",
            ["Add to vehicle catalog", "Create and park in the selected garage", "Cancel"]);
        if (mode is null or 2) return;
        if (mode == 1 && state.Garage is null)
        {
            ChooseGarage();
            if (state.Garage is null) return;
        }
        int? spot = mode == 1 ? ChooseSpot(null) : null;
        if (mode == 1 && spot is null) return;
        VehicleCreationResult result = state.Service.VehicleCreation.FromJson(
            path,
            mode == 1 ? state.Garage : null,
            spot == 0 ? null : spot);
        SetStatus(result.Message);
        if (!result.Success)
            dialogs.Message("Could not import vehicle", result.Message);
        else
            RevealVehicle(result.Vehicle!);
        RefreshRows();
    }

    private int? ChooseSpot(Vehicle? vehicle)
    {
        if (state.Garage is null) return null;

        var spotDetails = state.Garage.SpotDetails;
        List<int> available = vehicle is null
            ? spotDetails.Where(s => !s.IsOccupied).Select(s => s.SpotNumber).ToList()
            : state.Garage.AvailableSpotsFor(vehicle).ToList();

        if (available.Count == 0)
        {
            dialogs.Message("No available spots",
                "No open spot can accept this vehicle. Choose another garage.");
            return null;
        }

        List<string> choices = ["First available suitable spot", .. available.Select(number =>
        {
            var spot = spotDetails[number - 1];
            return $"Spot {number} / " + (spot.Dimensions is null ? "No size limit" :
                $"{spot.Dimensions.LengthMeters} x {spot.Dimensions.WidthMeters} x {spot.Dimensions.HeightMeters} m");
        })];
        int? selected = dialogs.Choose("Choose parking spot", choices);
        return selected is null ? null : selected == 0 ? 0 : available[selected.Value - 1];
    }

    private void ParkVehicle()
    {
        if (state.Garage is null) return;

        Vehicle? vehicle = SelectedVehicle;
        if (vehicle is null)
        {
            IReadOnlyList<Vehicle> available = state.Service.GetParkableVehicles(state.Garage);
            int? pick = dialogs.Choose("Choose a vehicle to park",
                available.Select(item => $"{item.RegistrationNumber} / {item.Manufacturer} {item.Model}")
                    .ToList());
            if (pick is null) return;
            vehicle = available[pick.Value];
        }

        if (state.Service.IsParked(vehicle))
        {
            dialogs.Message("Already parked",
                "Remove this vehicle from its current garage before parking it elsewhere.");
            return;
        }

        int? spot = ChooseSpot(vehicle);
        if (spot is null) return;

        ParkingResult result = state.Service.Park(vehicle, state.Garage, spot == 0 ? null : spot);
        SetStatus(result.Message);
        if (!result.Success)
            dialogs.Message("Parking", result.Message);
        RefreshRows();
    }

    private void RemoveVehicle()
    {
        if (SelectedVehicle is not Vehicle vehicle) return;

        VehicleGarage? garage = state.Location(vehicle);
        if (garage is null)
        {
            dialogs.Message("Vehicle is available", "This vehicle is already outside the garages.");
            return;
        }

        if (!dialogs.Confirm("Remove from garage",
            $"Remove {vehicle.RegistrationNumber} from {garage.GarageName}?\n" +
            "The vehicle stays in your catalog.", "_Remove"))
            return;

        SetStatus(garage.Remove(vehicle.RegistrationNumber).Message);
        RefreshRows();
    }

    private void ParkingActions()
    {
        int? action = dialogs.Choose("Parking tools",
            ["View spot availability and sizes", "Create a vehicle and park it", "Show vehicle type counts"]);

        if (action == 1)
            CreateVehicle(true);
        else if (action == 0 && state.Garage is not null)
            ShowParkingSpots(state.Garage);
        else if (action == 2 && state.Garage is not null)
            ShowVehicleTypeCounts(state.Garage);
    }

    private void ShowParkingSpots(VehicleGarage garage)
    {
        string description = string.Join("\n", garage.SpotDetails.Select(spot =>
            $"Spot {spot.SpotNumber,-5} {(spot.IsOccupied ? "Occupied" : "Available"),-11} " +
            (spot.Dimensions is null
                ? "No size limit"
                : $"{spot.Dimensions.LengthMeters} x {spot.Dimensions.WidthMeters} x " +
                  $"{spot.Dimensions.HeightMeters} m")));

        dialogs.Message($"{garage.GarageName} / Parking spots", description);
    }

    private void ShowVehicleTypeCounts(VehicleGarage garage)
    {
        string summary = string.Join("\n", garage.VehiclesByType.Select(group =>
            $"{group.VehicleType}: {group.Vehicles.Count}"));
        dialogs.Message("Vehicle type counts",
            summary.Length > 0 ? summary : "No parked vehicles yet.");
    }

    private void Search()
    {
        if (state.Garage is null)
        {
            ChooseGarage();
            if (state.Garage is null) return;
        }

        int? action = dialogs.Choose("Search the selected garage",
            ["Set search filters", "Clear all filters and show all vehicles"]);
        if (action is null) return;

        var criteria = action == 1
            ? new Garage.Queries.VehicleSearchCriteria()
            : SearchForm.Show(dialogs, state.Criteria);
        if (criteria is null) return;

        state.Criteria = criteria;
        state.Page = WorkspacePage.Search;
        layout.Navigation.SelectedItem = 3;
        RefreshRows();
        SetStatus("Search updated. Use Find to narrow the results further.");
    }

    private void GarageResult(GarageCreationResult? result)
    {
        if (result is null) return;
        SetStatus(result.Message);
        if (result.Success)
        {
            state.Garage = result.Garage;
            state.Page = WorkspacePage.Parking;
            layout.Navigation.SelectedItem = 2;
            layout.Query.Text = "";
        }
        else dialogs.Message("Could not create garage", result.Message);
        RefreshRows();
    }

    private void RevealVehicle(Vehicle vehicle)
    {
        state.Page = WorkspacePage.Vehicles;
        state.QuickFilter = "";
        layout.Query.Text = "";
        layout.Navigation.SelectedItem = 1;
        RefreshRows();
        int row = displayedVehicles.ToList().FindIndex(item => ReferenceEquals(item, vehicle));
        if (row >= 0) layout.Table.SetSelection(0, row, false);
        layout.Table.SetFocus();
    }

    private void SetStatus(string message) => layout.Status.Text = message;
    private void Help() => dialogs.Message("Welcome to Garage Workspace",
        "NAVIGATION\nTab / Shift+Tab: move between controls across panels\nArrow keys: select rows and sidebar pages\nEnter: open a choice or inspect a row\nEsc: close the current dialog\n\nSHORTCUTS\nF1 Help  /  F2 New vehicle  /  F3 Search\nF4 New garage  /  F5 Refresh\nCtrl+F Find  /  Ctrl+Q Quit\n\nWORKING WITH FILES\nImport buttons open a file browser filtered to JSON.\nVehicle files contain one vehicle object; garage imports use a layout and optional placements.\n\nYour work stays in this session. Imported files are never changed.");

    private void Quit()
    {
        if (dialogs.Confirm("Close Garage Workspace", "Changes exist only in this session.\nClose the application?", "_Quit")) application.RequestStop(this);
    }

    private void Guard(Action action)
    {
        try { action(); }
        catch (Exception exception)
        {
            ErrorLogger.TryLog(exception, "Terminal workspace action");
            dialogs.Message("Unable to finish", "That action could not be completed.\n" + exception.Message);
        }
    }
}
