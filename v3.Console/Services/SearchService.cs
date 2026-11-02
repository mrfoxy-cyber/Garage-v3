using Garage.Manager;
using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using Garage.Manager.Interfaces;
using Garage.Manager.Queries;

namespace Garage.Terminal.Services;

public sealed class SearchService
{
    //private readonly IHandler handler;
    private readonly IGarageManager manager;

    public SearchService(IGarageManager manager)
    {
        this.manager = manager;
    }

    public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(string garageId, VehicleSearchCriteria criteria)
    {
        return manager.SearchVehicles(garageId, criteria);
    }

    public (IReadOnlyList<VehicleLocation> Vehicles, OperationResult Result) SearchAllVehicles(VehicleSearchCriteria criteria)
    {
        return manager.SearchAllVehicles(criteria);
    }   
}
