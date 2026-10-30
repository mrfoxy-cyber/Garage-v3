using Garage.Models;
using Garage.Persistence.Json;
using Garage.Results;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Services;

/// <summary>Creates valid vehicles and commits catalog registration only after optional parking succeeds.</summary>
public sealed class VehicleCreationService
{
    private readonly VehicleCatalog catalog;
    private readonly Func<Vehicle, VehicleGarage, int?, ParkingResult> park;

    internal VehicleCreationService(VehicleCatalog catalog, Func<Vehicle, VehicleGarage, int?, ParkingResult> park)
    {
        this.catalog = catalog;
        this.park = park;
    }

    public VehicleCreationResult Create(Func<Vehicle> factory, VehicleGarage? targetGarage = null, int? spotNumber = null)
    {
        ArgumentNullException.ThrowIfNull(factory);
        Vehicle vehicle;
        try
        {
            vehicle = factory();
            if (vehicle is null)
                return VehicleCreationResult.Failed("Vehicle creation returned no vehicle.");
        }
        catch (Exception exception) when (ExpectedFailure.IsExpected(exception))
        {
            return VehicleCreationResult.Failed(exception.Message, ExpectedFailure.Code(exception));
        }

        if (catalog.ContainsRegistration(vehicle.RegistrationNumber))
            return VehicleCreationResult.Failed(
                $"A vehicle with registration {vehicle.RegistrationNumber} already exists.", OperationError.DuplicateRegistration);
        if (spotNumber is not null && targetGarage is null)
            return VehicleCreationResult.Failed("Choose a garage before choosing a parking spot.");

        ParkingResult? parking = null;
        if (targetGarage is not null)
        {
            parking = park(vehicle, targetGarage, spotNumber);
            if (!parking.Success)
                return VehicleCreationResult.Failed(parking.Message, parking.Error);
        }

        catalog.Add(vehicle);
        return VehicleCreationResult.Completed(vehicle, parking, targetGarage is null ? null :
            $"Created {vehicle.GetType().Name} ({vehicle.RegistrationNumber}) and parked it in " +
            $"{targetGarage.GarageName} at spot {parking!.SpotNumber}.");
    }

    public VehicleCreationResult FromJson(string path, VehicleGarage? targetGarage = null, int? spotNumber = null) =>
        Create(() => VehicleJsonLoader.LoadVehicle(path), targetGarage, spotNumber);
}
