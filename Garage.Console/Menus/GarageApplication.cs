using Garage.ConsoleApp.Prompts;
using Garage.Models;
using Garage.Results;
using Garage.Services;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.ConsoleApp.Menus;

internal sealed class GarageApplication
{
    private readonly GarageService service;
    private readonly VehicleCreator vehicleCreator;

    public GarageApplication(GarageService service)
    {
        this.service = service;
        vehicleCreator = new VehicleCreator(service);
    }

    public void Run()
    {
        ConsoleMenu startMenu = new(
            title: "Garage system",
            options:
            [
                new MenuOption(
                    "Create and list vehicles",
                    vehicleCreator.ShowMenu),

                new MenuOption(
                    "Choose garage",
                    ChooseGarage),

                new MenuOption(
                    "Create garage (unrestricted spots)",
                    CreateGarage),

                new MenuOption(
                    "Create garage with sized parking spots",
                    CreateGarageWithSizedSpots),

                new MenuOption(
                    "Create garage from JSON files",
                    CreateGarageFromJson)
            ],
            backOptionText: "Exit");

        startMenu.Show();
    }

    private void ChooseGarage()
    {
        if (service.Garages.Count == 0)
        {
            Console.WriteLine("There are no garages yet.");
            ConsoleInput.Pause();
            return;
        }

        List<MenuOption> garageOptions = [];

        for (int i = 0; i < service.Garages.Count; i++)
        {
            VehicleGarage selectedGarage = service.Garages[i];

            var menuOption = new MenuOption(
                () => $"{selectedGarage.GarageName} ({selectedGarage.OccupiedSpaces}/{selectedGarage.Capacity})",
                () => OpenGarage(selectedGarage)
                );

            garageOptions.Add(menuOption);
        }



        ConsoleMenu garageMenu = new(
            title: "Choose garage",
            options: garageOptions);

        garageMenu.Show();
    }

    private void CreateGarage()
    {
        CreateGarage(useSpotDimensions: false);
    }

    private void CreateGarageWithSizedSpots()
    {
        CreateGarage(useSpotDimensions: true);
    }

    private void CreateGarageFromJson()
    {
        string configurationPath = ConsoleInput.ReadRequiredString(
            "Garage configuration JSON path: ");
        string? placementPath = ConsoleInput.ReadOptionalString(
            "Parking assignments JSON path (leave empty for an empty garage): ");

        GarageCreationResult result = service.CreateGarageFromJson(configurationPath, placementPath);
        Console.WriteLine(result.Message);
        ConsoleInput.Pause();
        if (result.Success)
            OpenGarage(result.Garage!);
    }

    private void CreateGarage(bool useSpotDimensions)
    {
        string name = ConsoleInput.ReadRequiredString(
            "Garage name: ");

        bool nameAlreadyExists = service.ContainsGarage(name);

        if (nameAlreadyExists)
        {
            Console.WriteLine(
                "A garage with that name already exists.");
            ConsoleInput.Pause();
            return;
        }

        int capacity = ConsoleInput.ReadInt(
            "Number of parking spots: ",
            minimum: 1,
            maximum: VehicleGarage.MaximumGarageCapacity);

        GarageCreationResult result;

        if (useSpotDimensions)
        {
            Console.WriteLine("Enter the size used by most parking spots:");
            Dimensions defaultSize = ReadSpotDimensions();
            List<Dimensions> sizes = Enumerable.Repeat(defaultSize, capacity).ToList();

            int customCount = ConsoleInput.ReadInt(
                "Number of spots with a different size: ",
                minimum: 0,
                maximum: capacity);

            for (int i = 0; i < customCount; i++)
            {
                int spotNumber = ConsoleInput.ReadInt(
                    "Spot number to customize: ",
                    minimum: 1,
                    maximum: capacity);

                Console.WriteLine($"Size for parking spot {spotNumber}:");
                sizes[spotNumber - 1] = ReadSpotDimensions();
            }

            result = service.CreateGarage(name, sizes);
        }
        else
        {
            result = service.CreateGarage(name, capacity);
        }

        Console.WriteLine(result.Message);
        ConsoleInput.Pause();
        if (result.Success)
            OpenGarage(result.Garage!);
    }

    private static Dimensions ReadSpotDimensions() => new(
        ConsoleInput.ReadDouble("Length in meters: ", 0.01, 1_000),
        ConsoleInput.ReadDouble("Width in meters: ", 0.01, 1_000),
        ConsoleInput.ReadDouble("Height in meters: ", 0.01, 1_000));

    private void OpenGarage(VehicleGarage garage)
    {
        GarageActions actions = new(garage, service);

        ConsoleMenu vehicleMenu = new(
            title: garage.GarageName,
            options:
            [
                new MenuOption(
                    "Create and park vehicle",
                    () => vehicleCreator.ShowMenuForGarage(garage)),

                new MenuOption(
                    "Create and park vehicle at a specific spot",
                    () => vehicleCreator.ShowMenuForGarageAtSpot(garage)),

                new MenuOption(
                    "Park a vehicle",
                    actions.ParkVehicle),

                new MenuOption(
                    "Park a vehicle to a specific spot",
                    actions.ParkVehicleAtSpot),

                new MenuOption(
                    "List parking spots and sizes",
                    actions.ListParkingSpots),

                new MenuOption(
                    "List all parked vehicles",
                    actions.ListParkedVehicles),

                new MenuOption(
                    "List vehicle types",
                    actions.ListVehicleTypes),

                new MenuOption(
                    "Find vehicle by registration number",
                    actions.FindVehicleByRegistration),

                new MenuOption(
                    "Search vehicles",
                    actions.SearchVehicles),

                new MenuOption(
                    "Remove vehicle",
                    actions.RemoveVehicle)
            ],
            backOptionText: "Back");

        vehicleMenu.Show();
    }
}
