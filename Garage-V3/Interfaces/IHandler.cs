using Garage_V3.queries;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage_V3.Interfaces;

public interface IHandler
{
    OperationResult CreateGarage(string name, int capacity);

    OperationResult RemoveVehicle(string registrationNumber);

    (IVehicle? Vehicle, OperationResult Result) FindVehicle(string registrationNumber);

    (GarageInfo? Info, OperationResult Result) GetGarageInfo();

    (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) GetVehicles();

    (IReadOnlyList<VehicleTypeCount> Counts, OperationResult Result) GetVehicleTypeCounts();

    ParkingResult ParkVehicle( IVehicle vehicle, int? spotNumber = null);

    (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(VehicleSearchCriteria criteria);
}