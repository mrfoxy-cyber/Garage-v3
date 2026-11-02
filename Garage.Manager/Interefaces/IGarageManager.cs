using Garage.Manager.Queries;
using Garage_V3.queries;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage.Manager.Interfaces;

public interface IGarageManager
{
    (string? GarageId, OperationResult Result) CreateGarage(string name, int capacity, IEnumerable<IVehicle>? initialVehicles = null);

    (IReadOnlyList<ManagedGarageInfo> Garages, OperationResult Result) GetGarages();

    (GarageInfo? Info, OperationResult Result) GetGarageInfo(string garageId);

    ParkingResult ParkVehicle(string garageId, IVehicle vehicle, int? spotNumber = null);

    OperationResult RemoveVehicle(string garageId, string registrationNumber);

    (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) GetVehicles(string garageId);

    (IReadOnlyList<VehicleTypeCount> Counts, OperationResult Result) GetVehicleTypeCounts(string garageId);

    (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(string garageId, VehicleSearchCriteria criteria);

    (VehicleLocation? Location, OperationResult Result) FindVehicle(string registrationNumber);

    (IReadOnlyList<VehicleLocation> Vehicles, OperationResult Result) SearchAllVehicles(VehicleSearchCriteria criteria);
}