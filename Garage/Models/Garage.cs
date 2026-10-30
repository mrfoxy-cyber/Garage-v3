using Garage.Queries;
using Garage.Results;
using Garage.Vehicles;
using Garage.Vehicles.Components;

namespace Garage.Models;

public sealed class Garage
{
    public const int MaximumGarageCapacity = 100_000;
    private readonly List<ParkingSpot> parkingSpots;

    public string GarageName { get; }
    public int Capacity { get; }
    public int OccupiedSpaces => parkingSpots.Count(spot => spot.Vehicle is not null);
    public int AvailableSpaces => Capacity - OccupiedSpaces;
    public IReadOnlyList<Vehicle> Vehicles => parkingSpots
        .Select(spot => spot.Vehicle).OfType<Vehicle>().ToList().AsReadOnly();

    public IReadOnlyList<ParkingSpotInfo> SpotDetails => parkingSpots
        .Select(spot => new ParkingSpotInfo(spot.Number, spot.Dimensions, spot.Vehicle is not null))
        .ToList().AsReadOnly();

    public IReadOnlyList<ParkedVehicleInfo> ParkedVehicles => parkingSpots
        .Where(spot => spot.Vehicle is not null)
        .Select(spot => new ParkedVehicleInfo(spot.Number, spot.Vehicle!)).ToList().AsReadOnly();

    public IReadOnlyList<VehicleTypeGroup> VehiclesByType => ParkedVehicles
        .GroupBy(parked => parked.Vehicle.GetType().Name)
        .Select(group => new VehicleTypeGroup(group.Key, group.ToList().AsReadOnly()))
        .ToList().AsReadOnly();

    public Garage(string garageName, int capacity) : this(garageName, capacity, [], null) { }

    public Garage(string garageName, List<Vehicle> vehicles)
        : this(garageName, vehicles?.Count ?? throw new ArgumentNullException(nameof(vehicles)), vehicles, null) { }

    public Garage(string garageName, int capacity, List<Vehicle> initialVehicles)
        : this(garageName, capacity, initialVehicles, null) { }

    public Garage(string garageName, IReadOnlyList<Dimensions> spotDimensions, List<Vehicle>? initialVehicles = null)
        : this(garageName, spotDimensions?.Count ?? throw new ArgumentNullException(nameof(spotDimensions)),
            initialVehicles ?? [], spotDimensions) { }

    private Garage(string garageName, int capacity, List<Vehicle> initialVehicles, IReadOnlyList<Dimensions>? spotDimensions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(garageName);
        if (capacity < 1 || capacity > MaximumGarageCapacity)
            throw new ArgumentOutOfRangeException(nameof(capacity),
                $"Capacity must be between 1 and {MaximumGarageCapacity}.");

        List<Vehicle> startingVehicles = initialVehicles ?? [];
        if (startingVehicles.Count > capacity)
            throw new ArgumentException("Too many initial vehicles.", nameof(initialVehicles));
        if (startingVehicles.Any(vehicle => vehicle is null))
            throw new ArgumentException("Initial vehicles must not contain null vehicles.", nameof(initialVehicles));
        if (startingVehicles.GroupBy(vehicle => vehicle.RegistrationNumber, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1))
            throw new ArgumentException("Initial vehicles contain duplicate registration numbers.", nameof(initialVehicles));
        if (spotDimensions is not null && spotDimensions.Any(size => size is null))
            throw new ArgumentException("Every parking spot needs dimensions.", nameof(spotDimensions));

        GarageName = garageName.Trim();
        Capacity = capacity;
        parkingSpots = Enumerable.Range(1, capacity)
            .Select(number => new ParkingSpot(number, spotDimensions?[number - 1])).ToList();

        foreach (Vehicle vehicle in startingVehicles)
        {
            ParkingResult result = Park(vehicle);
            if (!result.Success)
                throw new ArgumentException(
                    $"Initial vehicle {vehicle.RegistrationNumber} could not be parked: {result.Message}",
                    nameof(initialVehicles));
        }
    }

    public ParkedVehicleInfo? FindVehicleByRegistration(string registrationNumber) =>
        ParkedVehicles.FirstOrDefault(parked => string.Equals(
            parked.Vehicle.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<int> AvailableSpotsFor(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (RegistrationExists(vehicle.RegistrationNumber))
            return new List<int>().AsReadOnly();
        return parkingSpots.Where(spot => spot.Vehicle is null && spot.Fits(vehicle))
            .Select(spot => spot.Number).ToList().AsReadOnly();
    }

    public ParkingResult Park(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (RegistrationExists(vehicle.RegistrationNumber))
            return DuplicateRegistration(vehicle);
        ParkingSpot? availableSpot = parkingSpots.FirstOrDefault(spot => spot.Vehicle is null && spot.Fits(vehicle));
        if (availableSpot is null)
            return AvailableSpaces == 0
                ? ParkingResult.Failed("The garage is full.", OperationError.GarageFull)
                : ParkingResult.Failed("No available parking spot is large enough for this vehicle.", OperationError.VehicleDoesNotFit);
        return Park(vehicle, availableSpot.Number);
    }

    public ParkingResult Park(Vehicle vehicle, int spot)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (spot < 1 || spot > Capacity)
            return ParkingResult.Failed($"Parking spot must be between 1 and {Capacity}.", OperationError.SpotOutOfRange);
        ParkingSpot parkingSpot = parkingSpots[spot - 1];
        if (parkingSpot.Vehicle is not null)
            return ParkingResult.Failed($"Parking spot {spot} is occupied.", OperationError.SpotOccupied);
        if (!parkingSpot.Fits(vehicle))
            return ParkingResult.Failed(
                $"Vehicle {vehicle.RegistrationNumber} does not fit in parking spot {spot}.", OperationError.VehicleDoesNotFit);
        if (RegistrationExists(vehicle.RegistrationNumber))
            return DuplicateRegistration(vehicle);

        parkingSpot.Park(vehicle);
        return ParkingResult.Completed(spot, $"Vehicle {vehicle.RegistrationNumber} parked at spot {spot}.");
    }

    public GarageOperationResult Remove(string registrationNumber)
    {
        ParkedVehicleInfo? match = FindVehicleByRegistration(registrationNumber);
        if (match is null)
            return GarageOperationResult.Failed("We could not find your vehicle.", OperationError.VehicleNotFound);
        parkingSpots[match.SpotNumber - 1].Clear();
        return GarageOperationResult.Completed(match.SpotNumber,
            $"Vehicle {match.Vehicle.RegistrationNumber} was removed from parking spot {match.SpotNumber}.");
    }

    public GarageOperationResult RemoveAtSpot(int spotNumber)
    {
        if (spotNumber < 1 || spotNumber > Capacity)
            return GarageOperationResult.Failed(
                $"Parking spot must be between 1 and {Capacity}.", OperationError.SpotOutOfRange);
        Vehicle? vehicle = parkingSpots[spotNumber - 1].Vehicle;
        return vehicle is null
            ? GarageOperationResult.Failed($"Parking spot {spotNumber} is already empty.", OperationError.SpotEmpty)
            : Remove(vehicle.RegistrationNumber);
    }

    public IReadOnlyList<Vehicle> Search(VehicleSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        return Vehicles.Where(vehicle => VehicleSearch.Matches(vehicle, criteria)).ToList().AsReadOnly();
    }

    private bool RegistrationExists(string registrationNumber) => parkingSpots.Any(spot =>
        string.Equals(spot.Vehicle?.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));

    private static ParkingResult DuplicateRegistration(Vehicle vehicle) => ParkingResult.Failed(
        $"A vehicle with registration number {vehicle.RegistrationNumber} is already parked.",
        OperationError.DuplicateRegistration);
}
