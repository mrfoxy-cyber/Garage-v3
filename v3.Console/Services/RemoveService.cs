using Garage.Manager;
using Garage_V3.Interfaces;
using Garage_V3.results;
using Garage.Manager.Interfaces;

namespace Garage.Terminal.Services;

public sealed class RemoveService
{
    //private readonly IHandler handler;
    private readonly IGarageManager manager;

    public RemoveService(IGarageManager manager)
    {
        this.manager = manager;
    }

    public OperationResult RemoveVehicle(string garageId, string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return OperationResult.Failed(
                "Enter a registration number.",
                OperationError.ValidationFailed);
        }

        return manager.RemoveVehicle(garageId, registrationNumber.Trim());
    }
}
