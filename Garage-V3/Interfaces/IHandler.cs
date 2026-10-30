using Garage_V3.queries;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage_V3.Interfaces;

public interface IHandler
{
    OperationResult CreateGarage(string name, int capacity);

    ParkingResult ParkVehicle(
        IVehicle vehicle,
        int? spotNumber = null);

    OperationResult RemoveVehicle(string registrationNumber);

    IVehicle? FindVehicle(string registrationNumber);

    IReadOnlyList<ParkedVehicleInfo> GetVehicles();

    IReadOnlyList<ParkedVehicleInfo> SearchVehicles(
        VehicleSearchCriteria criteria);

    IReadOnlyList<VehicleTypeCount> GetVehicleTypeCounts();

    GarageInfo? GetGarageInfo();
}