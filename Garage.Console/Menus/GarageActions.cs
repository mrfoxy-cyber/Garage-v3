using Garage.ConsoleApp.Prompts;
using Garage.ConsoleApp.Display;
using Garage.Queries;
using Garage.Models;
using Garage.Services;
using Garage.Results;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.ConsoleApp.Menus;

internal sealed class GarageActions
{
    private readonly VehicleGarage garage;
    private readonly GarageService service;

    public GarageActions(VehicleGarage garage, GarageService service)
    {
        this.garage = garage;
        this.service = service;
    }

    public void ParkVehicle()
    {
        if (garage.AvailableSpaces == 0)
        {
            Console.WriteLine("The garage is full.");
            ConsoleInput.Pause();
            return;
        }

        Vehicle? vehicle = SelectVehicle();

        if (vehicle is not null)
            Park(vehicle);
    }

    public void ParkVehicleAtSpot()
    {
        if (garage.AvailableSpaces == 0)
        {
            Console.WriteLine("The garage is full.");
            ConsoleInput.Pause();
            return;
        }

        ShowOpenSpots();

        int spot = ConsoleInput.ReadInt(
            "Choose parking spot: ",
            minimum: 1,
            maximum: garage.Capacity);

        if (garage.ParkedVehicles.Any(parked => parked.SpotNumber == spot))
        {
            Console.WriteLine($"Parking spot {spot} is occupied.");
            ConsoleInput.Pause();
            return;
        }

        Vehicle? vehicle = SelectVehicle(spot);

        if (vehicle is not null)
            Park(vehicle, spot);
    }

    public void ListParkedVehicles()
    {
        var parkedVehicles = garage.ParkedVehicles;

        if (parkedVehicles.Count == 0)
        {
            Console.WriteLine("The garage is empty.");
            ConsoleInput.Pause();
            return;
        }

        foreach (var parkedVehicle in parkedVehicles)
        {
            if (parkedVehicle.Vehicle is null)
            {
                continue;
            } 
            Console.WriteLine(
                $"Spot {parkedVehicle.SpotNumber}: " +
                $"{parkedVehicle.Vehicle.GetType().Name}, " +
                $"{parkedVehicle.Vehicle.RegistrationNumber}, " +
                $"{parkedVehicle.Vehicle.Manufacturer} " +
                $"{parkedVehicle.Vehicle.Model}");
        }

        ConsoleInput.Pause();
    }

    public void ListParkingSpots()
    {
        foreach (var spot in garage.SpotDetails)
        {
            string size = spot.Dimensions is null
                ? "no size limit"
                : $"{spot.Dimensions.LengthMeters} x " +
                  $"{spot.Dimensions.WidthMeters} x " +
                  $"{spot.Dimensions.HeightMeters} m";

            Console.WriteLine(
                $"Spot {spot.SpotNumber}: " +
                $"{(spot.IsOccupied ? "occupied" : "open")}, {size}");
        }

        ConsoleInput.Pause();
    }

    private void ShowOpenSpots()
    {
        Console.WriteLine("Open parking spots:");

        foreach (var spot in garage.SpotDetails.Where(spot => !spot.IsOccupied))
        {
            string size = spot.Dimensions is null
                ? "no size limit"
                : $"{spot.Dimensions.LengthMeters} x " +
                  $"{spot.Dimensions.WidthMeters} x " +
                  $"{spot.Dimensions.HeightMeters} m";

            Console.WriteLine($"{spot.SpotNumber}. {size}");
        }
    }

    public void ListVehicleTypes()
    {
        bool foundVehicle = false;

        foreach (var group in garage.VehiclesByType)
        {
            foundVehicle = true;
            Console.WriteLine(
                $"{group.VehicleType}: {group.Vehicles.Count}");
        }

        if (!foundVehicle)
        {
            Console.WriteLine("The garage is empty.");
        }

        ConsoleInput.Pause();
    }

    public void FindVehicleByRegistration()
    {
        string registrationNumber =
            ConsoleInput.ReadRequiredString(
                "Registration number: ");

        var match = garage
            .FindVehicleByRegistration(registrationNumber);

        if (match is null)
        {
            Console.WriteLine("No vehicle was found.");
        }
        else
        {
            VehicleDisplay.PrintVehicle(
                match.SpotNumber,
                match.Vehicle);
        }

        ConsoleInput.Pause();
    }

    public void RemoveVehicle()
    {
        string registrationNumber =
            ConsoleInput.ReadRequiredString(
                "Registration number: ");

        GarageOperationResult result =
            garage.Remove(registrationNumber);

        Console.WriteLine(result.Message);
        ConsoleInput.Pause();
    }

    public void SearchVehicles()
    {
        SearchCriteriaCreator criteriaCreator = new();

        ConsoleMenu searchMenu = new(
            title: "Search vehicles",
            options:
            [
                new MenuOption(
                    () => $"Vehicle types: {criteriaCreator.VehicleTypesText}",
                    criteriaCreator.ChooseVehicleTypes),

                new MenuOption(
                    () => $"Color: {criteriaCreator.ColorText}",
                    criteriaCreator.ChooseColor),

                new MenuOption(
                    () => $"Number of wheels: {criteriaCreator.NumberOfWheelsText}",
                    criteriaCreator.ChooseNumberOfWheels),

                new MenuOption(
                    () => $"Number of engines: {criteriaCreator.NumberOfEnginesText}",
                    criteriaCreator.ChooseNumberOfEngines),

                new MenuOption(
                    () => $"Cylinder volume (cc): {criteriaCreator.CylinderVolumeText}",
                    criteriaCreator.ChooseCylinderVolume),

                new MenuOption(
                    () => $"Fuel type: {criteriaCreator.FuelTypeText}",
                    criteriaCreator.ChooseFuelType),

                new MenuOption(
                    () => $"Number of seats: {criteriaCreator.NumberOfSeatsText}",
                    criteriaCreator.ChooseNumberOfSeats),

                new MenuOption(
                    () => $"Length (m): {criteriaCreator.LengthText}",
                    criteriaCreator.ChooseLength),

                new MenuOption(
                    () => $"Registration number: {criteriaCreator.RegistrationNumberText}",
                    criteriaCreator.ChooseRegistrationNumber),

                new MenuOption(
                    () => $"Manufacturer: {criteriaCreator.ManufacturerText}",
                    criteriaCreator.ChooseManufacturer),

                new MenuOption(
                    () => $"Model: {criteriaCreator.ModelText}",
                    criteriaCreator.ChooseModel),

                new MenuOption(
                    () => $"Manufacturing year: {criteriaCreator.ManufacturingYearText}",
                    criteriaCreator.ChooseManufacturingYear),

                new MenuOption(
                    "Vehicle-specific filters",
                    () => ShowVehicleSpecificFilters(criteriaCreator)),

                new MenuOption(
                    "Run search",
                    () => ShowSearchResults(criteriaCreator.Create())),

                new MenuOption(
                    "Clear all filters",
                    criteriaCreator.Clear)
            ]);

        searchMenu.Show();
    }

    private static void ShowVehicleSpecificFilters(SearchCriteriaCreator criteriaCreator)
    {
        ConsoleMenu menu = new(
            title: "Vehicle-specific filters",
            options:
            [
                new MenuOption(
                    () => $"Car body type: {criteriaCreator.BodyTypeText}",
                    criteriaCreator.ChooseBodyType),
                new MenuOption(
                    () => $"Motorcycle type: {criteriaCreator.MotorcycleTypeText}",
                    criteriaCreator.ChooseMotorcycleType),
                new MenuOption(
                    () => $"Motorcycle has sidecar: {criteriaCreator.HasSidecarText}",
                    criteriaCreator.ChooseHasSidecar),
                new MenuOption(
                    () => $"Boat type: {criteriaCreator.BoatTypeText}",
                    criteriaCreator.ChooseBoatType),
                new MenuOption(
                    () => $"Boat draft (m): {criteriaCreator.DraftText}",
                    criteriaCreator.ChooseDraft),
                new MenuOption(
                    () => $"Aircraft wingspan (m): {criteriaCreator.WingspanText}",
                    criteriaCreator.ChooseWingspan),
                new MenuOption(
                    () => $"Aircraft maximum altitude (m): {criteriaCreator.MaximumAltitudeText}",
                    criteriaCreator.ChooseMaximumAltitude),
                new MenuOption(
                    () => $"Bus standing capacity: {criteriaCreator.StandingPassengerCapacityText}",
                    criteriaCreator.ChooseStandingPassengerCapacity),
                new MenuOption(
                    () => $"Truck axles: {criteriaCreator.NumberOfAxlesText}",
                    criteriaCreator.ChooseNumberOfAxles),
                new MenuOption(
                    () => $"Truck maximum load (kg): {criteriaCreator.MaximumLoadText}",
                    criteriaCreator.ChooseMaximumLoad)
            ]);

        menu.Show();
    }

    private void ShowSearchResults(VehicleSearchCriteria criteria)
    {
        IReadOnlyList<Vehicle> matches = garage.Search(criteria);

        ConsoleScreen.Clear();
        Console.WriteLine($"Found {matches.Count} vehicle(s).");
        Console.WriteLine();

        for (int index = 0; index < matches.Count; index++)
        {
            Vehicle vehicle = matches[index];
            var parkedVehicle = garage.FindVehicleByRegistration(
                vehicle.RegistrationNumber);

            if (parkedVehicle is not null)
            {
                VehicleDisplay.PrintVehicle(
                    parkedVehicle.SpotNumber,
                    vehicle);
            }
        }

        ConsoleInput.Pause();
    }

    private void Park(Vehicle vehicle)
    {
        ParkingResult result = service.Park(vehicle, garage);

        Console.WriteLine();
        Console.WriteLine(result.Message);
        ConsoleInput.Pause();
    }

    private void Park(Vehicle vehicle, int spot)
    {
        ParkingResult result = service.Park(vehicle, garage, spot);

        Console.WriteLine();
        Console.WriteLine(result.Message);
        ConsoleInput.Pause();
    }

    private Vehicle? SelectVehicle(int? spotNumber = null)
    {
        IReadOnlyList<Vehicle> vehicles = service.GetParkableVehicles(garage, spotNumber);

        if (vehicles.Count == 0)
        {
            Console.WriteLine(
                spotNumber is int requestedSpot
                    ? $"No available vehicles fit in parking spot {requestedSpot}."
                    : "No available vehicles fit in an open parking spot.");
            ConsoleInput.Pause();
            return null;
        }

        Console.WriteLine("Choose a vehicle:");
        Console.WriteLine("0. Cancel");

        for (int index = 0; index < vehicles.Count; index++)
        {
            Vehicle vehicle = vehicles[index];
            Console.WriteLine(
                $"{index + 1}. {vehicle.GetType().Name}, " +
                $"{vehicle.Manufacturer} {vehicle.Model}, " +
                $"{vehicle.RegistrationNumber}");
        }

        int choice = ConsoleInput.ReadInt(
            "Choose: ", 0, vehicles.Count);

        return choice == 0
            ? null
            : vehicles[choice - 1];
    }

}
