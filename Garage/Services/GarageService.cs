using Garage.Models;
using Garage.Persistence.Json;
using Garage.Results;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Services;

/// <summary>Owns the garage collection, vehicle catalog, and operations spanning multiple garages.</summary>
public sealed class GarageService
{
    private readonly List<VehicleGarage> garages = [];

    public GarageService(List<VehicleGarage> initialGarages, List<Vehicle> initialVehicles)
    {
        ArgumentNullException.ThrowIfNull(initialGarages);
        ArgumentNullException.ThrowIfNull(initialVehicles);
        Catalog = new VehicleCatalog(initialVehicles);
        foreach (VehicleGarage garage in initialGarages)
        {
            GarageCreationResult result = RegisterGarage(garage);
            if (!result.Success)
                throw new ArgumentException(result.Message, nameof(initialGarages));
        }
        VehicleCreation = new VehicleCreationService(Catalog, ParkNewVehicle);
    }

    public IReadOnlyList<VehicleGarage> Garages => garages.AsReadOnly();
    public VehicleCatalog Catalog { get; }
    public VehicleCreationService VehicleCreation { get; }

    public bool ContainsGarage(string name) => garages.Any(garage =>
        string.Equals(garage.GarageName, name.Trim(), StringComparison.OrdinalIgnoreCase));

    public GarageCreationResult CreateGarage(string name, int capacity) =>
        CreateAndRegister(() => new VehicleGarage(name, capacity));

    public GarageCreationResult CreateGarage(string name, IReadOnlyList<Dimensions> sizes) =>
        CreateAndRegister(() => new VehicleGarage(name, sizes));

    public GarageCreationResult CreateGarageFromJson(string configurationPath, string? placementPath = null) =>
        CreateAndRegister(() => GarageJsonLoader.Load(configurationPath, placementPath));

    public bool IsParked(Vehicle vehicle) => garages.Any(garage => garage.Vehicles.Any(parked =>
        string.Equals(parked.RegistrationNumber, vehicle.RegistrationNumber, StringComparison.OrdinalIgnoreCase)));

    public IReadOnlyList<Vehicle> GetParkableVehicles(VehicleGarage garage, int? spotNumber = null)
    {
        if (!garages.Contains(garage))
            return new List<Vehicle>().AsReadOnly();
        return Catalog.Vehicles.Where(vehicle => !IsParked(vehicle) &&
            (spotNumber is int spot
                ? garage.AvailableSpotsFor(vehicle).Contains(spot)
                : garage.AvailableSpotsFor(vehicle).Count > 0)).ToList().AsReadOnly();
    }

    public ParkingResult Park(Vehicle vehicle, VehicleGarage garage, int? spotNumber = null)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        ArgumentNullException.ThrowIfNull(garage);
        if (!Catalog.Vehicles.Contains(vehicle))
            return ParkingResult.Failed("Choose a vehicle from this application's catalog.", OperationError.VehicleNotFound);
        return ParkNewVehicle(vehicle, garage, spotNumber);
    }

    private ParkingResult ParkNewVehicle(Vehicle vehicle, VehicleGarage garage, int? spotNumber)
    {
        if (!garages.Contains(garage))
            return ParkingResult.Failed("Choose a garage registered with this application.", OperationError.ValidationFailed);
        if (IsParked(vehicle))
            return ParkingResult.Failed(
                $"Vehicle {vehicle.RegistrationNumber} is already parked in a garage.", OperationError.VehicleAlreadyParked);
        return spotNumber is int spot ? garage.Park(vehicle, spot) : garage.Park(vehicle);
    }

    private GarageCreationResult CreateAndRegister(Func<VehicleGarage> factory)
    {
        try
        {
            return RegisterGarage(factory());
        }
        catch (Exception exception) when (ExpectedFailure.IsExpected(exception))
        {
            return GarageCreationResult.Failed(exception.Message, ExpectedFailure.Code(exception));
        }
    }

    private GarageCreationResult RegisterGarage(VehicleGarage garage)
    {
        ArgumentNullException.ThrowIfNull(garage);
        if (ContainsGarage(garage.GarageName))
            return GarageCreationResult.Failed("A garage with that name already exists.", OperationError.DuplicateGarageName);
        foreach (Vehicle vehicle in garage.Vehicles)
        {
            if (IsParked(vehicle) || Catalog.ContainsRegistration(vehicle.RegistrationNumber))
                return GarageCreationResult.Failed(
                    $"A vehicle with registration {vehicle.RegistrationNumber} already exists.",
                    OperationError.DuplicateRegistration);
        }

        foreach (Vehicle vehicle in garage.Vehicles)
            Catalog.Add(vehicle);
        garages.Add(garage);
        return GarageCreationResult.Completed(garage);
    }
}
