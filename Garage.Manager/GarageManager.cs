using Garage.Manager.Queries;
using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using Garage_V3.Services;
using Vehicles.Interfaces;
using Garage.Manager.Interfaces;

namespace Garage.Manager;

public sealed class GarageManager: IGarageManager
{
    private sealed record Entry(string Name, IHandler Handler);

    private readonly Dictionary<string, Entry> garages = [];

    private readonly Dictionary<string, string> vehicleLocations =
        new(StringComparer.OrdinalIgnoreCase);

    public (string? GarageId, OperationResult Result) CreateGarage(string name, int capacity, IEnumerable<IVehicle>? initialVehicles = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, OperationResult.Failed(
                "Enter a garage name.",
                OperationError.ValidationFailed));
        }

        string garageName = name.Trim();

        if (garages.Values.Any(entry => string.Equals(entry.Name, garageName, StringComparison.OrdinalIgnoreCase)))
        {
            return (null, OperationResult.Failed(
                $"A garage named '{garageName}' already exists.",
                OperationError.GarageAlreadyExists));
        }

        IHandler candidate = new GarageHandler();

        var creationResult = candidate.CreateGarage(garageName, capacity);

        if (!creationResult.Success)
            return (null, creationResult);

        if (initialVehicles is not null)
        {
            var initializationResult = candidate.InitializeVehicles(initialVehicles);

            if (!initializationResult.Success)
                return (null, initializationResult);
        }

        var (vehicles, vehiclesResult) = candidate.GetVehicles();

        if (!vehiclesResult.Success)
            return (null, vehiclesResult);

        var conflicts = vehicles
            .Select(entry => entry.Vehicle.RegistrationNumber.Trim())
            .Where(registration => vehicleLocations.ContainsKey(registration))
            .ToList();

        if (conflicts.Count > 0)
        {
            return (null, OperationResult.Failed(
                $"Vehicles are already registered in another garage: {string.Join(", ", conflicts)}.",
                OperationError.DuplicateRegistration));
        }

        string id = Guid.NewGuid().ToString("N");

        garages.Add(id, new Entry(garageName, candidate));

        foreach (var entry in vehicles)
            vehicleLocations.Add(entry.Vehicle.RegistrationNumber.Trim(), id);

        return (id, OperationResult.Completed($"Garage '{garageName}' created."));
    }

    public (IReadOnlyList<ManagedGarageInfo> Garages, OperationResult Result) GetGarages()
    {
        List<ManagedGarageInfo> items = [];

        foreach (var entry in garages)
        {
            var (info, result) = entry.Value.Handler.GetGarageInfo();

            if (!result.Success)
                return (Array.Empty<ManagedGarageInfo>(), result);

            if (info is null)
            {
                return (Array.Empty<ManagedGarageInfo>(), OperationResult.Failed(
                    "Garage information could not be retrieved.",
                    OperationError.UnexpectedError));
            }

            items.Add(new ManagedGarageInfo(entry.Key, info));
        }

        return (items.AsReadOnly(), OperationResult.Completed($"Found {items.Count} garages."));
    }

    public (GarageInfo? Info, OperationResult Result) GetGarageInfo(string garageId)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return (null, MissingGarage());

        return handler.GetGarageInfo();
    }

    public ParkingResult ParkVehicle(string garageId, IVehicle vehicle, int? spotNumber = null)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return ParkingResult.Failed("Choose an existing garage.", OperationError.NoGarage);

        if (vehicle is null || string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
        {
            return ParkingResult.Failed(
                "Choose a vehicle with a registration number.",
                OperationError.ValidationFailed);
        }

        string registration = vehicle.RegistrationNumber.Trim();

        if (vehicleLocations.TryGetValue(registration, out string? existingGarageId))
        {
            string garageName = garages[existingGarageId].Name;

            return ParkingResult.Failed(
                $"Vehicle '{registration}' is already parked in garage '{garageName}'.",
                OperationError.DuplicateRegistration);
        }

        var result = handler.ParkVehicle(vehicle, spotNumber);

        if (result.Success)
            vehicleLocations.Add(registration, garageId);

        return result;
    }

    public OperationResult RemoveVehicle(string garageId, string registrationNumber)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return MissingGarage();

        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return OperationResult.Failed(
                "Enter a registration number.",
                OperationError.ValidationFailed);
        }

        string registration = registrationNumber.Trim();

        var result = handler.RemoveVehicle(registration);

        if (result.Success)
            vehicleLocations.Remove(registration);

        return result;
    }

    public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) GetVehicles(string garageId)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return (Array.Empty<ParkedVehicleInfo>(), MissingGarage());

        return handler.GetVehicles();
    }

    public (IReadOnlyList<VehicleTypeCount> Counts, OperationResult Result) GetVehicleTypeCounts(string garageId)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return (Array.Empty<VehicleTypeCount>(), MissingGarage());

        return handler.GetVehicleTypeCounts();
    }

    public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(string garageId, VehicleSearchCriteria criteria)
    {
        IHandler? handler = FindHandler(garageId);

        if (handler is null)
            return (Array.Empty<ParkedVehicleInfo>(), MissingGarage());

        return handler.SearchVehicles(criteria);
    }

    private IHandler? FindHandler(string garageId)
    {
        if (string.IsNullOrWhiteSpace(garageId))
            return null;

        return garages.TryGetValue(garageId, out var entry) ? entry.Handler : null;
    }

    private static OperationResult MissingGarage()
    {
        return OperationResult.Failed("Choose an existing garage.", OperationError.NoGarage);
    }

    public (VehicleLocation? Location, OperationResult Result) FindVehicle(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return (null, OperationResult.Failed(
                "Enter a registration number.",
                OperationError.ValidationFailed));
        }

        string registration = registrationNumber.Trim();

        if (!vehicleLocations.TryGetValue(registration, out var garageId))
        {
            return (null, OperationResult.Failed(
                $"No vehicle found with registration '{registration}'.",
                OperationError.VehicleNotFound));
        }

        if (!garages.TryGetValue(garageId, out var garage))
        {
            return (null, OperationResult.Failed(
                "The vehicle's garage could not be found.",
                OperationError.UnexpectedError));
        }

        var (vehicles, result) = garage.Handler.GetVehicles();

        if (!result.Success)
            return (null, result);

        var matches = vehicles
            .Where(entry => string.Equals(
                entry.Vehicle.RegistrationNumber,
                registration,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            return (null, OperationResult.Failed(
                "The vehicle location is inconsistent with the garage contents.",
                OperationError.UnexpectedError));
        }

        if (matches.Count > 1)
        {
            return (null, OperationResult.Failed(
                $"Multiple vehicles have registration '{registration}'.",
                OperationError.MultipleVehiclesFound));
        }

        var parked = matches[0];

        VehicleLocation location = new(
            GarageId: garageId,
            GarageName: garage.Name,
            SpotNumber: parked.SpotNumber,
            Vehicle: parked.Vehicle);

        return (location, OperationResult.Completed("Vehicle found."));
    }

    public (IReadOnlyList<VehicleLocation> Vehicles, OperationResult Result) SearchAllVehicles(VehicleSearchCriteria criteria)
    {
        if (criteria is null)
        {
            return (Array.Empty<VehicleLocation>(), OperationResult.Failed(
                "Supply search criteria.",
                OperationError.ValidationFailed));
        }

        List<VehicleLocation> matches = [];

        foreach (var garage in garages)
        {
            var (vehicles, result) = garage.Value.Handler.SearchVehicles(criteria);

            if (!result.Success)
            {
                return (Array.Empty<VehicleLocation>(), OperationResult.Failed(
                    $"Could not search garage '{garage.Value.Name}': {result.Message}",
                    result.Error ?? OperationError.UnexpectedError));
            }

            foreach (var entry in vehicles)
            {
                matches.Add(new VehicleLocation(
                    GarageId: garage.Key,
                    GarageName: garage.Value.Name,
                    SpotNumber: entry.SpotNumber,
                    Vehicle: entry.Vehicle));
            }
        }

        return (matches.AsReadOnly(), OperationResult.Completed(
            $"Found {matches.Count} matching vehicles across all garages."));
    }
}