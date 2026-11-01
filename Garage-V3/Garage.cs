using Garage.Vehicles;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage_V3
{


    public sealed class Garage<T> : IEnumerable<T>
    where T : class, IVehicle
    {
        public const int MaximumGarageCapacity = 100_000;

        public string GarageName { get; }
        public int Capacity { get; }

        private readonly List<(int SpotNumber, T Vehicle)> vehicles = [];

        public IReadOnlyList<(int SpotNumber, T Vehicle)> Vehicles => vehicles.AsReadOnly();

        public int OccupiedSpaces => Vehicles.Count;
        public int AvailableSpaces => Capacity - OccupiedSpaces;

        public int? FindAvailableSpot()
        {
            var occupiedSpots = Vehicles.Select(entry => entry.SpotNumber).ToHashSet();

            for (int spot = 1; spot <= Capacity; spot++)
            {
                if (!occupiedSpots.Contains(spot))
                    return spot;
            }

            return null;
        }

        public Garage(string garageName, int capacity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(garageName);

            if (capacity < 1 || capacity > MaximumGarageCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            GarageName = garageName.Trim();
            Capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator() => Vehicles.Select(entry => entry.Vehicle).GetEnumerator();

        System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator() =>
                GetEnumerator();


        public ParkingResult ParkVehicle(T vehicle, int? spotNumber = null)
        {
            if (vehicle is null || string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
            {
                return ParkingResult.Failed("Choose a vehicle with a registration number.", OperationError.ValidationFailed);
            }

            bool alreadyParked = vehicles.Any(entry =>
                string.Equals(entry.Vehicle.RegistrationNumber, vehicle.RegistrationNumber, StringComparison.OrdinalIgnoreCase));

            if (alreadyParked)
            {
                return ParkingResult.Failed("A vehicle with that registration is already parked.", OperationError.DuplicateRegistration);
            }

            if (spotNumber is int requestedSpot && (requestedSpot < 1 || requestedSpot > Capacity))
            {
                return ParkingResult.Failed($"Choose a spot between 1 and {Capacity}.", OperationError.InvalidSpot);
            }

            if (spotNumber is int chosenSpot && vehicles.Any(entry => entry.SpotNumber == chosenSpot))
            {
                return ParkingResult.Failed($"Spot {chosenSpot} is occupied.", OperationError.SpotOccupied);
            }

            if (AvailableSpaces < 1)
            {
                return ParkingResult.Failed("Garage is full.", OperationError.GarageFull);
            }

            int? selectedSpot = spotNumber ?? FindAvailableSpot();

            if (selectedSpot is null)
            {
                return ParkingResult.Failed("No available parking spot was found.", OperationError.GarageFull);
            }

            vehicles.Add((SpotNumber: selectedSpot.Value, Vehicle: vehicle));

            return ParkingResult.Completed(selectedSpot.Value);
        }

        public OperationResult RemoveVehicle(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return OperationResult.Failed(
                    "Enter a registration number.",
                    OperationError.ValidationFailed);
            }

            string registration = registrationNumber.Trim();

            var matches = vehicles.FindAll(entry =>
                string.Equals(entry.Vehicle.RegistrationNumber, registration, StringComparison.OrdinalIgnoreCase));

            if (matches.Count > 1)
            {
                return OperationResult.Failed(
                    $"Multiple vehicles have registration '{registration}'.",
                    OperationError.MultipleVehiclesFound);
            }

            if (matches.Count == 0)
            {
                return OperationResult.Failed(
                    $"No vehicle found with registration '{registration}'.",
                    OperationError.VehicleNotFound);
            }

            vehicles.Remove(matches[0]);

            return OperationResult.Completed("Vehicle removed.");
        }
    }

}
