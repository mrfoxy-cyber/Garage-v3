using Garage.ConsoleApp.Menus;
using Garage.Services;
using Garage.Persistence.Json;
using Garage.Diagnostics;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;


try
{
    List<Vehicle> parkedVehicles =
        VehicleJsonLoader.LoadVehicles("vehicles.json");

    List<Vehicle> createdVehicles =
        VehicleJsonLoader.LoadVehicles("created-vehicles.json");

    VehicleGarage startingGarage = new(
       garageName: "Lexicon Garage",
       capacity: 100,
        initialVehicles: parkedVehicles);

    GarageService service = new([startingGarage], createdVehicles);
    GarageApplication application = new(service);


    application.Run();

    Console.WriteLine("Goodbye!");
}

  catch (Exception exception) when (
    exception is IOException or
    System.Text.Json.JsonException or
    ArgumentException or
    NotSupportedException)
{
    Console.WriteLine(
        $"The garage could not be started: {exception.Message}");
}
catch (Exception exception)
{
    ErrorLogger.TryLog(exception);

    Console.WriteLine(
        "An unexpected error occurred.");

    Environment.ExitCode = 1;
}


