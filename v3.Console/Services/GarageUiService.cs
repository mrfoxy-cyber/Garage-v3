using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Views;
using Garage_V3.queries;
using Garage_V3.results;
using Garage.Manager.Interfaces;

namespace Garage.Terminal.Services;

public sealed class GarageUiService
{
    private readonly IGarageManager manager;
    private readonly MainWindow window;
    private readonly ParkService parkService;
    private readonly RemoveService removeService;
    private readonly SearchService searchService;

    private HomeView? homeView;
    private string? selectedGarageId;
    private Action? refreshCurrentPage;

    public GarageUiService(IGarageManager manager, MainWindow window)
    {
        this.manager = manager;
        this.window = window;

        parkService = new(manager);
        removeService = new(manager);
        searchService = new(manager);

        window.MenuRequested += HandleMenuRequest;

        ShowHome();
    }

    private void ShowHome()
    {
        var (garages, result) = manager.GetGarages();

        if (!result.Success)
        {
            window.Home.CreateInput.ShowError(result.Message);
            window.ShowPage(window.Home);
            return;
        }

        var choices = garages
            .Select(entry => new GarageChoice(
                Id: entry.Id,
                Name: entry.Info.GarageName,
                VehicleTypes: "Mixed",
                Capacity: entry.Info.Capacity,
                OccupiedSpaces: entry.Info.OccupiedSpaces))
            .ToList();

        homeView = new(window.Theme, choices);

        homeView.CreateInput.CreateRequested += CreateGarage;
        homeView.LoadInput.LoadRequested += LoadGarage;

        foreach (var card in homeView.GarageCards)
        {
            card.OpenRequested += id => ShowGarage(id);
            card.ParkRequested += id => ShowParking(id, () => ShowHome());
            card.RemoveRequested += id => ShowRemoveVehicle(id, () => ShowHome());
            card.SearchRequested += garageId =>
                        ShowSearch(garageId, () => ShowHome());
        }

        selectedGarageId = null;
        refreshCurrentPage = ShowHome;

        window.ShowPage(homeView);
    }

    private void CreateGarage(string name, int capacity)
    {
        var (garageId, result) = manager.CreateGarage(name, capacity);

        if (!result.Success || garageId is null)
        {
            homeView?.CreateInput.ShowError(result.Message);
            return;
        }

        ShowGarage(garageId, result);
    }

    private GarageChoice? GetGarageChoice(string garageId)
    {
        var (info, result) = manager.GetGarageInfo(garageId);

        if (!result.Success || info is null)
        {
            ShowHome();
            homeView?.CreateInput.ShowError(result.Message);
            return null;
        }

        return new GarageChoice(
            Id: garageId,
            Name: info.GarageName,
            VehicleTypes: "Mixed",
            Capacity: info.Capacity,
            OccupiedSpaces: info.OccupiedSpaces);
    }

    private void ShowGarage(string garageId, OperationResult? feedback = null)
    {
        GarageChoice? garage = GetGarageChoice(garageId);

        if (garage is null)
            return;

        GarageView page = new(window.Theme, garage);

        page.BackRequested += ShowHome;

        page.Summary.OpenRequested += id => ShowGarage(id);
        page.Summary.ParkRequested += id => ShowParking(id, () => ShowGarage(id));
        page.Summary.RemoveRequested += id => ShowRemoveVehicle(id, () => ShowGarage(id));
        page.Summary.SearchRequested += id => ShowSearch(id, () => ShowGarage(id));

        if (feedback is not null)
            page.Feedback.ShowResult(feedback);

        var (vehicles, vehiclesResult) = manager.GetVehicles(garageId);

        if (vehiclesResult.Success)
            page.Vehicles.ShowVehicles(vehicles);
        else
            page.Feedback.ShowResult(vehiclesResult);

        var (counts, countsResult) = manager.GetVehicleTypeCounts(garageId);

        if (countsResult.Success)
            page.TypeCounts.ShowCounts(counts);
        else
            page.Feedback.ShowResult(countsResult);

        selectedGarageId = garageId;
        refreshCurrentPage = () => ShowGarage(garageId);

        window.ShowPage(page);
    }

    private void ShowGarages()
    {
        var (garages, result) = manager.GetGarages();

        if (!result.Success)
        {
            ShowHome();
            homeView?.CreateInput.ShowError(result.Message);
            return;
        }

        var choices = garages
            .Select(entry => new GarageChoice(
                Id: entry.Id,
                Name: entry.Info.GarageName,
                VehicleTypes: "Mixed",
                Capacity: entry.Info.Capacity,
                OccupiedSpaces: entry.Info.OccupiedSpaces))
            .ToList();

        GaragesView page = new(window.Theme);

        page.Picker.ShowGarages(choices);
        page.Picker.OpenRequested += id => ShowGarage(id);
        page.Picker.CreateRequested += ShowHome;
        page.BackRequested += ShowHome;

        selectedGarageId = null;
        refreshCurrentPage = ShowGarages;

        window.ShowPage(page);
    }

    private void ShowParking(string garageId, Action returnToPreviousPage)
    {
        GarageChoice? garage = GetGarageChoice(garageId);

        if (garage is null)
            return;

        ParkVehicleView page = new(window.Theme, garage);

        page.BackRequested += returnToPreviousPage;

        page.Input.ParkRequested += request =>
        {
            var result = parkService.ParkVehicle(garageId, request);

            page.Feedback.ShowResult(result);

            if (!result.Success)
            {
                page.Input.ShowError(result.Message);
                return;
            }

            page.Input.Reset();
        };

        selectedGarageId = garageId;
        refreshCurrentPage = null;

        window.ShowPage(page);
    }

    private void ShowSearch(string garageId, Action returnToPreviousPage)
    {
        GarageChoice? garage = GetGarageChoice(garageId);

        if (garage is null)
            return;

        SearchVehiclesView page = new(window.Theme, garage);

        page.BackRequested += returnToPreviousPage;

        page.Input.SearchRequested += criteria =>
            SearchVehicles(page, garage, criteria);

        page.Input.ClearRequested += () =>
            SearchVehicles(page, garage, new VehicleSearchCriteria());

        selectedGarageId = garageId;
        refreshCurrentPage = page.Input.Submit;

        window.ShowPage(page);

        SearchVehicles(page, garage, new VehicleSearchCriteria());
    }

    private void SearchVehicles(SearchVehiclesView page, GarageChoice garage, VehicleSearchCriteria criteria)
    {
        page.Input.ShowError(null);

        var (vehicles, result) = searchService.SearchVehicles(garage.Id, criteria);

        page.Feedback.ShowResult(result);

        if (!result.Success)
        {
            page.Input.ShowError(result.Message);
            page.Results.ShowVehicles([]);
            return;
        }

        var matches = vehicles
            .Select(entry => new LocatedVehicleInfo(
                GarageId: garage.Id,
                GarageName: garage.Name,
                SpotNumber: entry.SpotNumber,
                Vehicle: entry.Vehicle))
            .ToList();

        page.Results.ShowVehicles(matches);
    }

    private void ShowRemoveVehicle(string garageId, Action returnToPreviousPage)
    {
        GarageChoice? garage = GetGarageChoice(garageId);

        if (garage is null)
            return;

        RemoveVehicleView page = new(window.Theme, garage);

        page.BackRequested += returnToPreviousPage;

        page.Input.LookupRequested += registration =>
            ShowRemovalConfirmation(garage, page, registration);

        selectedGarageId = garageId;
        refreshCurrentPage = null;

        window.ShowPage(page);
    }

    private void ShowRemovalConfirmation(GarageChoice garage, RemoveVehicleView page, string registration)
    {
        page.Input.ShowError(null);

        VehicleSearchCriteria criteria = new()
        {
            RegistrationNumber = registration
        };

        var (vehicles, result) = searchService.SearchVehicles(garage.Id, criteria);

        page.Feedback.ShowResult(result);

        if (!result.Success)
        {
            page.Input.ShowError(result.Message);
            return;
        }

        if (vehicles.Count == 0)
        {
            page.Input.ShowError($"No vehicle found with registration '{registration}'.");
            return;
        }

        if (vehicles.Count > 1)
        {
            page.Input.ShowError($"Multiple vehicles have registration '{registration}'.");
            return;
        }

        var entry = vehicles[0];

        LocatedVehicleInfo match = new(
            GarageId: garage.Id,
            GarageName: garage.Name,
            SpotNumber: entry.SpotNumber,
            Vehicle: entry.Vehicle);

        ConfirmRemovalView confirmation = new(window.Theme, match);

        confirmation.BackRequested += () => window.ShowPage(page);
        confirmation.Confirmation.CancelRequested += () => window.ShowPage(page);

        confirmation.Confirmation.ConfirmRequested += selected =>
            ConfirmRemoval(selected, confirmation, (result) =>
            {
                page.Input.Registration.Text = string.Empty;
                page.Input.ShowError(null);
                page.Feedback.ShowResult(result);
                window.ShowPage(page);
            });

        window.ShowPage(confirmation);
    }

    private void ConfirmRemoval(LocatedVehicleInfo match, ConfirmRemovalView confirmation, Action<OperationResult> onRemoved)
    {
        var result = removeService.RemoveVehicle(match.GarageId, match.Vehicle.RegistrationNumber);

        confirmation.Feedback.ShowResult(result);

        if (!result.Success)
        {
            confirmation.Confirmation.ShowError(result.Message);
            return;
        }

        confirmation.Confirmation.Clear();
        onRemoved(result);
    }

    private void HandleMenuRequest(GarageMenuAction action)
    {
        switch (action)
        {
            case GarageMenuAction.Home:
            case GarageMenuAction.CreateGarage:
                ShowHome();
                return;

            case GarageMenuAction.RemoveVehicle:
                ShowGlobalRemoveVehicle();
                return;

            case GarageMenuAction.SelectGarage:
                ShowGarages();
                return;

            case GarageMenuAction.Refresh:
                refreshCurrentPage?.Invoke();
                return;

            case GarageMenuAction.Quit:
                return;

            case GarageMenuAction.FindVehicle:
            case GarageMenuAction.SearchVehicles:
                ShowGlobalSearchVehicles();
                return;
        }

        if (selectedGarageId is null)
        {
            ShowGarages();
            return;
        }

        string garageId = selectedGarageId;

        switch (action)
        {
            case GarageMenuAction.ParkVehicle:
                ShowParking(garageId, () => ShowGarage(garageId));
                break;

            case GarageMenuAction.FindVehicle:
            case GarageMenuAction.SearchVehicles:
                ShowSearch(garageId, () => ShowGarage(garageId));
                break;

            case GarageMenuAction.ShowTypeCounts:
                ShowGarage(garageId);
                break;
        }
    }

    private void ShowRemovalConfirmation(LocatedVehicleInfo match, Action returnToLookup, Action<OperationResult> onRemoved)
    {
        ConfirmRemovalView confirmation = new(window.Theme, match);

        confirmation.BackRequested += returnToLookup;
        confirmation.Confirmation.CancelRequested += returnToLookup;

        confirmation.Confirmation.ConfirmRequested += selected =>
            ConfirmRemoval(selected, confirmation, onRemoved);

        window.ShowPage(confirmation);
    }

    private void ShowGlobalRemoveVehicle()
    {
        GlobalRemoveVehicleView page = new(window.Theme);

        page.BackRequested += ShowHome;

        page.Input.LookupRequested += registration =>
            ShowGlobalRemovalConfirmation(page, registration);

        selectedGarageId = null;
        refreshCurrentPage = null;

        window.ShowPage(page);
    }

    private void ShowGlobalRemovalConfirmation(GlobalRemoveVehicleView page, string registration)
    {
        page.Input.ShowError(null);

        var (location, result) = manager.FindVehicle(registration);

        page.Feedback.ShowResult(result);

        if (!result.Success || location is null)
        {
            page.Input.ShowError(result.Message);
            return;
        }

        LocatedVehicleInfo match = new(
            GarageId: location.GarageId,
            GarageName: location.GarageName,
            SpotNumber: location.SpotNumber,
            Vehicle: location.Vehicle);

        ShowRemovalConfirmation(
            match,
            () => window.ShowPage(page),
            removalResult =>
            {
                page.Input.Registration.Text = string.Empty;
                page.Input.ShowError(null);
                page.Feedback.ShowResult(removalResult);
                window.ShowPage(page);
            });
    }

    private void ShowGlobalSearchVehicles()
    {
        GlobalSearchVehiclesView page = new(window.Theme);

        page.BackRequested += ShowHome;

        page.Input.SearchRequested += criteria =>
            SearchAllVehicles(page, criteria);

        page.Input.ClearRequested += () =>
            SearchAllVehicles(page, new VehicleSearchCriteria());

        selectedGarageId = null;
        refreshCurrentPage = page.Input.Submit;

        window.ShowPage(page);

        SearchAllVehicles(page, new VehicleSearchCriteria());
    }

    private void SearchAllVehicles(GlobalSearchVehiclesView page, VehicleSearchCriteria criteria)
    {
        page.Input.ShowError(null);

        var (vehicles, result) = searchService.SearchAllVehicles(criteria);

        page.Feedback.ShowResult(result);

        if (!result.Success)
        {
            page.Input.ShowError(result.Message);
            page.Results.ShowVehicles([]);
            return;
        }

        var matches = vehicles
            .Select(entry => new LocatedVehicleInfo(
                GarageId: entry.GarageId,
                GarageName: entry.GarageName,
                SpotNumber: entry.SpotNumber,
                Vehicle: entry.Vehicle))
            .ToList();

        page.Results.ShowVehicles(matches);
    }

    private void LoadGarage()
    {
        if (window.App is not { } app)
            return;

        homeView?.LoadInput.ShowError(null);

        string? path = GarageFilePicker.Choose(app);

        if (path is null)
            return;

        var (garageId, result) = manager.LoadGarage(path);

        if (!result.Success || garageId is null)
        {
            homeView?.LoadInput.ShowError(result.Message);
            return;
        }

        ShowGarage(garageId, result);
    }
}
