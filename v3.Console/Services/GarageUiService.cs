using Garage.Terminal.Contracts;
using Garage.Terminal.Views;
using Garage_V3.Interfaces;

namespace Garage.Terminal.Services;

public sealed class GarageUiService
{
    private readonly IHandler handler;
    private readonly MainWindow window;

    private readonly ParkService parkService;

    public GarageUiService(IHandler handler, MainWindow window)
    {
        this.handler = handler;
        this.window = window;
        this.parkService = new ParkService(handler);

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

        garageView.Summary.ParkRequested += _ =>
            ShowParking(garage, () => window.ShowPage(garageView), () => RefreshGarage(garageView, homeView));

        foreach (var card in homeView.GarageCards)
        {
            card.OpenRequested += _ => window.ShowPage(garageView);

            card.ParkRequested += _ =>
                ShowParking(garage, () => window.ShowPage(homeView), () => RefreshGarage(garageView, homeView));
        }

        window.ShowPage(garageView);


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
