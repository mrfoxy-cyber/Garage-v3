using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using System.Linq.Expressions;
using System;
using Vehicles.Interfaces;
using Garage.Vehicles;

namespace Garage_V3.Services
{
    internal class GarageHandler : IHandler
    {
        private Garage<IVehicle>? garage;
        public OperationResult CreateGarage(string name, int capacity)
        {
            if (garage is not null)
            {
                return OperationResult.Failed(
                    "A garage already exists.",
                    OperationError.GarageAlreadyExists);
            }

            try
            {
                garage = new Garage<IVehicle>(name, capacity);

                return OperationResult.Completed("Garage created.");
            }
            catch (ArgumentException exception)
            {
                return OperationResult.Failed(
                    exception.Message,
                    OperationError.ValidationFailed);
            }
        }

        public ParkingResult ParkVehicle(IVehicle vehicle, int? spotNumber = null)
        {
            if (garage is null)
            {
                return ParkingResult.Failed("Create a garage first.", OperationError.NoGarage);
            }

            try
            {
                return garage.ParkVehicle(vehicle, spotNumber);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return ParkingResult.Failed("An unexpected error occurred while parking the vehicle.", OperationError.UnexpectedError);
            }
        }

        public (IVehicle? Vehicle, OperationResult Result) FindVehicle(
            string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return (null, OperationResult.Failed(
                    "Enter a registration number.",
                    OperationError.ValidationFailed));
            }

            if (garage is null)
            {
                return (null, OperationResult.Failed(
                    "Create a garage first.",
                    OperationError.NoGarage));
            }

            string registration = registrationNumber.Trim();

            IVehicle? vehicle;

            try
            {
                vehicle = garage.SingleOrDefault(vehicle =>
                    string.Equals(
                        vehicle.RegistrationNumber,
                        registration,
                        StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException)
            {
                return (null, OperationResult.Failed(
                    $"Multiple vehicles have registration '{registration}'.",
                    OperationError.MultipleVehiclesFound));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return (null, OperationResult.Failed("An unexpected error occurred while finding the vehicle.", OperationError.UnexpectedError));
            }

            if (vehicle is null)
            {
                return (null, OperationResult.Failed(
                    $"No vehicle found with registration '{registration}'.",
                    OperationError.VehicleNotFound));
            }

            return (vehicle, OperationResult.Completed("Vehicle found."));
        }

        public (GarageInfo? Info, OperationResult Result) GetGarageInfo()
        {
            if (garage is null)
            {
                return (null, OperationResult.Failed(
                    "Create a garage first.",
                    OperationError.NoGarage));
            }

            try
            {
                var info = new GarageInfo(
                    GarageName: garage.GarageName,
                    Capacity: garage.Capacity,
                    OccupiedSpaces: garage.OccupiedSpaces);

                return (info, OperationResult.Completed(
                    "Garage information retrieved."));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return (null, OperationResult.Failed(
                    "An unexpected error occurred while retrieving garage information.",
                    OperationError.UnexpectedError));
            }
        }

        public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) GetVehicles()
        {
            if (garage is null)
            {
                return (
                    Vehicles: Array.Empty<ParkedVehicleInfo>(),
                    Result: OperationResult.Failed(
                        "Create a garage first.",
                        OperationError.NoGarage)
                );
            }

            try
            {
                var vehicles = garage.Vehicles
                    .Select(entry => new ParkedVehicleInfo(
                        SpotNumber: entry.SpotNumber,
                        Vehicle: entry.Vehicle))
                    .ToList()
                    .AsReadOnly();

                return (
                    Vehicles: vehicles,
                    Result: OperationResult.Completed(
                        $"Found {vehicles.Count} parked vehicles.")
                );
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return (
                    Vehicles: Array.Empty<ParkedVehicleInfo>(),
                    Result: OperationResult.Failed(
                        "An unexpected error occurred while retrieving parked vehicles.",
                        OperationError.UnexpectedError)
                );
            }
        }

        public (IReadOnlyList<VehicleTypeCount> Counts, OperationResult Result) GetVehicleTypeCounts()
        {
            if (garage is null)
            {
                return (Array.Empty<VehicleTypeCount>(), OperationResult.Failed("Create a garage first.", OperationError.NoGarage));
            }

            try
            {
                var counts = garage
                    .GroupBy(vehicle => vehicle.GetType().Name)
                    .Select(group => new VehicleTypeCount(VehicleType: group.Key, Count: group.Count()))
                    .OrderBy(item => item.VehicleType)
                    .ToList()
                    .AsReadOnly();

                return (counts, OperationResult.Completed("Vehicle type counts retrieved."));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return (Array.Empty<VehicleTypeCount>(), OperationResult.Failed("An unexpected error occurred while retrieving vehicle type counts.", OperationError.UnexpectedError));
            }
        }

        public OperationResult RemoveVehicle(string registrationNumber)
        {
            if (garage is null)
            {
                return OperationResult.Failed("Create a garage first.", OperationError.NoGarage);
            }

            try
            {
                return garage.RemoveVehicle(registrationNumber);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return OperationResult.Failed("An unexpected error occurred while removing the vehicle.", OperationError.UnexpectedError);
            }
        }

        public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(VehicleSearchCriteria criteria)
        {
            if (criteria is null)
            {
                return (Array.Empty<ParkedVehicleInfo>(), OperationResult.Failed("Supply search criteria.", OperationError.ValidationFailed));
            }

            if (garage is null)
            {
                return (Array.Empty<ParkedVehicleInfo>(), OperationResult.Failed("Create a garage first.", OperationError.NoGarage));
            }

            try
            {
                var vehicles = garage.Vehicles
                    .Where(entry => MatchesCriteria(entry.Vehicle, criteria))
                    .Select(entry => new ParkedVehicleInfo(SpotNumber: entry.SpotNumber, Vehicle: entry.Vehicle))
                    .ToList()
                    .AsReadOnly();

                return (vehicles, OperationResult.Completed($"Found {vehicles.Count} matching vehicles."));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);

                return (Array.Empty<ParkedVehicleInfo>(), OperationResult.Failed("An unexpected error occurred while searching vehicles.", OperationError.UnexpectedError));
            }
        }

        private static bool MatchesCriteria(IVehicle vehicle, VehicleSearchCriteria criteria)
        {
            return MatchesText(vehicle.RegistrationNumber, criteria.RegistrationNumber)
                && MatchesText(vehicle.GetType().Name, criteria.VehicleType)
                && (criteria.CountryCode is null || vehicle.CountryCode == criteria.CountryCode)
                && (criteria.Color is null || (vehicle is Vehicle details && details.Color == criteria.Color))
                && (criteria.NumberOfWheels is null || (vehicle is IHasWheels wheels && wheels.NumberOfWheels == criteria.NumberOfWheels));
        }

        private static bool MatchesText(string? actual, string? expected) =>
            string.IsNullOrWhiteSpace(expected) ||
            string.Equals(
                actual,
                expected.Trim(),
                StringComparison.OrdinalIgnoreCase);
    }
}
