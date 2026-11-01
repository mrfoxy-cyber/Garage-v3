using Garage.Terminal.Contracts;
using Garage.Terminal.Views;
using Garage_V3.Interfaces;

namespace Garage.Terminal.Services;

public sealed class GarageUiService
{
    private readonly IHandler handler;
    private readonly MainWindow window;

    private readonly ParkService parkService;
    private readonly RemoveService removeService;

    public GarageUiService(IHandler handler, MainWindow window)
    {
        this.handler = handler;
        this.window = window;
        this.parkService = new ParkService(handler);
        this.removeService = new RemoveService(handler);


        window.Home.CreateInput.CreateRequested += CreateGarage;
    }
    // This method is called when the user requests to create a new garage.
    private void CreateGarage(string name, int capacity)
    {
        var result = handler.CreateGarage(name, capacity);

        if (!result.Success)
        {
            window.Home.CreateInput.ShowError(result.Message);
            return;
        }

        // After creating the garage, we want to show the garage view with the current garage information.
        var (info, infoResult) = handler.GetGarageInfo();

        if (!infoResult.Success || info is null)
        {
            window.Home.CreateInput.ShowError(infoResult.Message);
            return;
        }

        // Create a GarageChoice object to represent the current garage.

        GarageChoice garage = new(
            Id: "current",
            Name: info.GarageName,
            VehicleTypes: "Mixed",
            Capacity: info.Capacity,
            OccupiedSpaces: info.OccupiedSpaces);

        GarageView garageView = new(window.Theme, garage);
        HomeView homeView = new(window.Theme, [garage]);

        // The current handler owns one garage.
        homeView.CreateInput.Enabled = false;

        foreach (var card in homeView.GarageCards)
        {
            card.OpenRequested += _ => window.ShowPage(garageView);
        }

        // When the user requests to go back from the garage view, we show the home view again.
        garageView.BackRequested += () => window.ShowPage(homeView);


        // When the user requests to park a vehicle or remove a vehicle, we show the respective views.
        garageView.Summary.ParkRequested += _ =>
            ShowParking(garage, () => window.ShowPage(garageView), () => RefreshGarage(garageView, homeView));

        // When the user requests to remove a vehicle, we show the remove vehicle view.
        garageView.Summary.RemoveRequested += _ =>
            ShowRemoveVehicle(garage, () => window.ShowPage(garageView), () => RefreshGarage(garageView, homeView));

        // When the user requests to go back from the garage view, we show the home view again.
        foreach (var card in homeView.GarageCards)
        {
            card.OpenRequested += _ => window.ShowPage(garageView);

            card.ParkRequested += _ =>
                ShowParking(garage, () => window.ShowPage(homeView), () => RefreshGarage(garageView, homeView));

            card.RemoveRequested += _ =>
                ShowRemoveVehicle(garage, () => window.ShowPage(homeView), () => RefreshGarage(garageView, homeView));
        }


        // Show the garage view after creating the garage.

        window.ShowPage(garageView);
    }

    private void ShowParking(GarageChoice garage, Action returnToPreviousPage, Action refreshGarage)
    {
        ParkVehicleView page = new(window.Theme, garage);

        page.BackRequested += returnToPreviousPage;

        page.Input.ParkRequested += request =>
        {
            var result = parkService.ParkVehicle(request);

            page.Feedback.ShowResult(result);

            if (!result.Success)
            {
                page.Input.ShowError(result.Message);
                return;
            }

            page.Input.Reset();
            refreshGarage();
        };

        window.ShowPage(page);
    }

    private void ShowRemoveVehicle(GarageChoice garage, Action returnToPreviousPage, Action refreshGarage)
    {
        RemoveVehicleView page = new(window.Theme, garage);

        page.BackRequested += returnToPreviousPage;

        page.Input.LookupRequested += registration =>
            ShowRemovalConfirmation(garage, page, registration, refreshGarage);

        window.ShowPage(page);
    }

    private void ShowRemovalConfirmation(GarageChoice garage, RemoveVehicleView page, string registration, Action refreshGarage)
    {
        page.Input.ShowError(null);

        var (vehicle, findResult) = handler.FindVehicle(registration);

        if (!findResult.Success || vehicle is null)
        {
            page.Input.ShowError(findResult.Message);
            page.Feedback.ShowResult(findResult);
            return;
        }

        var (vehicles, vehiclesResult) = handler.GetVehicles();

        if (!vehiclesResult.Success)
        {
            page.Input.ShowError(vehiclesResult.Message);
            page.Feedback.ShowResult(vehiclesResult);
            return;
        }
        // Find the spot number of the vehicle in the garage.
        int? spotNumber = vehicles
            .Where(entry => string.Equals(
                entry.Vehicle.RegistrationNumber,
                vehicle.RegistrationNumber,
                StringComparison.OrdinalIgnoreCase))
            .Select(entry => (int?)entry.SpotNumber)
            .FirstOrDefault();

        if (spotNumber is null)
        {
            page.Input.ShowError("The vehicle is no longer parked. Search again.");
            return;
        }
        // Create a LocatedVehicleInfo object to represent the vehicle and its location in the garage.
        LocatedVehicleInfo match = new(
            GarageId: garage.Id,
            GarageName: garage.Name,
            SpotNumber: spotNumber.Value,
            Vehicle: vehicle);

        // Show a confirmation view to the user before removing the vehicle.
        ConfirmRemovalView confirmation = new(window.Theme, match);

        confirmation.BackRequested += () => window.ShowPage(page);
        confirmation.Confirmation.CancelRequested += () => window.ShowPage(page);

        // When the user confirms the removal, we call the ConfirmRemoval method to handle the removal process.
        confirmation.Confirmation.ConfirmRequested += selected =>
            ConfirmRemoval(selected, confirmation, page, refreshGarage);

        window.ShowPage(confirmation);
    }

    private void ConfirmRemoval(LocatedVehicleInfo match, ConfirmRemovalView confirmation, RemoveVehicleView page, Action refreshGarage)
    {
        // Call the RemoveService to remove the vehicle from the garage.!!!!!!!!
        var result = removeService.RemoveVehicle(match.Vehicle.RegistrationNumber);

        confirmation.Feedback.ShowResult(result);

        if (!result.Success)
        {
            confirmation.Confirmation.ShowError(result.Message);
            return;
        }

        confirmation.Confirmation.Clear();

        page.Input.Registration.Text = string.Empty;
        page.Input.ShowError(null);
        page.Feedback.ShowResult(result);

        refreshGarage();
        window.ShowPage(page);
    }

    private void RefreshGarage(GarageView garageView, HomeView homeView)
    {
        var (info, infoResult) = handler.GetGarageInfo();

        if (!infoResult.Success || info is null)
        {
            garageView.Feedback.ShowResult(infoResult);
            return;
        }

        GarageChoice garage = new(
            Id: "current",
            Name: info.GarageName,
            VehicleTypes: "Mixed",
            Capacity: info.Capacity,
            OccupiedSpaces: info.OccupiedSpaces);

        garageView.Summary.Update(garage);

        foreach (var card in homeView.GarageCards)
            card.Update(garage);

        var (vehicles, vehiclesResult) = handler.GetVehicles();

        if (vehiclesResult.Success)
            garageView.Vehicles.ShowVehicles(vehicles);
        else
            garageView.Feedback.ShowResult(vehiclesResult);

        var (counts, countsResult) = handler.GetVehicleTypeCounts();

        if (countsResult.Success)
            garageView.TypeCounts.ShowCounts(counts);
        else
            garageView.Feedback.ShowResult(countsResult);
    }
}
