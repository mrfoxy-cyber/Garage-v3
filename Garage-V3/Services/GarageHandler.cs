using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage_V3.Services
{
    internal class GarageHandler : IHandler
    {
{
        public OperationResult CreateGarage(string name, int capacity)
        {
            throw new NotImplementedException();
        }

        public IVehicle? FindVehicle(string registrationNumber)
        {
            throw new NotImplementedException();
        }

        public GarageInfo? GetGarageInfo()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ParkedVehicleInfo> GetVehicles()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<VehicleTypeCount> GetVehicleTypeCounts()
        {
            throw new NotImplementedException();
        }

        public ParkingResult ParkVehicle(IVehicle vehicle, int? spotNumber = null)
        {
            throw new NotImplementedException();
        }

        public OperationResult RemoveVehicle(string registrationNumber)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ParkedVehicleInfo> SearchVehicles(VehicleSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
}
