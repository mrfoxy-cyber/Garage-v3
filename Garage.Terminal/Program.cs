using Garage.Diagnostics;
using Garage.Persistence.Json;
using Garage.Services;
using Garage.Terminal.Presentation;
using Terminal.Gui.App;
using VehicleGarage = Garage.Models.Garage;

try
{
    if (args.Contains("--help"))
    {
        Console.WriteLine("Garage Workspace\nRun in a terminal with at least 80 columns and 24 rows.\n--empty starts without sample vehicles.\nF1 opens keyboard help. Changes stay in the current session.");
        return;
    }
    GarageService service = args.Contains("--empty")
        ? new GarageService([], [])
        : new GarageService(
            [new VehicleGarage("Lexicon Garage", 100, VehicleJsonLoader.LoadVehicles("vehicles.json"))],
            VehicleJsonLoader.LoadVehicles("created-vehicles.json"));

    using IApplication app = Application.Create();
    app.Init();
    using GarageWorkspace workspace = new(app, service);
    app.Run(workspace);
}
catch (Exception exception)
{
    ErrorLogger.TryLog(exception, "Start terminal workspace");
    Console.Error.WriteLine($"Garage Workspace could not start: {exception.Message}");
    Environment.ExitCode = 1;
}
