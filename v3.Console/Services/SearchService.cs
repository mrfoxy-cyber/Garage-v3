using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;

namespace Garage.Terminal.Services;

public sealed class SearchService
{
    private readonly IHandler handler;

    public SearchService(IHandler handler)
    {
        this.handler = handler;
    }

    public (IReadOnlyList<ParkedVehicleInfo> Vehicles, OperationResult Result) SearchVehicles(VehicleSearchCriteria criteria)
    {
        return handler.SearchVehicles(criteria);
    }
}
