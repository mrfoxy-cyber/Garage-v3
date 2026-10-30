using Garage.Factories;
using Garage.Vehicles;

namespace Garage.ConsoleApp.Display;

internal static class VehicleDisplay
{
    public static void PrintVehicle(int spotNumber, Vehicle vehicle)
    {
        Console.WriteLine($"Spot: {spotNumber}");
        Console.WriteLine($"Type: {vehicle.GetType().Name}");
        Console.WriteLine($"Registration number: {vehicle.RegistrationNumber}");
        Console.WriteLine($"Country of registration: {vehicle.CountryCode}");
        Console.WriteLine($"Vehicle: {vehicle.Manufacturer} {vehicle.Model}");
        Console.WriteLine($"Year: {vehicle.ManufacturingYear}");
        Console.WriteLine($"Color: {vehicle.Color}");
        Console.WriteLine();
        Console.WriteLine("------ Engine Information ------");

        if (vehicle.SpecificationDetails.Engines.Count == 0)
        {
            Console.WriteLine("No engines.");
        }
        else
        {
            foreach (var engine in vehicle.SpecificationDetails.Engines)
            {
                EngineDisplay.PrintEngine(engine);
                Console.WriteLine();
            }
        }
    }
}
