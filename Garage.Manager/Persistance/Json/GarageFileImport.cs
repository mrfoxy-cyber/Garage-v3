using System.Text.Json;
using Garage.Manager.Interfaces;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage.Manager.Persistence.Json;

internal static class GarageFileImport
{
    public static (string? GarageId, OperationResult Result) Load(string path, IGarageManager manager)
    {
        int vehicleNumber = 0;

        try
        {
            GarageFileData data = GarageJsonLoader.Read(path);
            List<IVehicle> vehicles = [];

            foreach (var entry in data.Vehicles)
            {
                vehicleNumber++;
                vehicles.Add(GarageJsonVehicleFactory.Create(entry));
            }

            return manager.CreateGarage(data.Name, data.Capacity, vehicles);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException or ArgumentException or NotSupportedException)
        {
            string context = vehicleNumber > 0 ? $"Vehicle {vehicleNumber}: " : string.Empty;

            return (null, OperationResult.Failed(
                $"Could not load garage. {context}{exception.Message}",
                OperationError.ValidationFailed));
        }
    }
}
