using Garage_V3.Interfaces;
using Garage_V3.results;

namespace Garage.Terminal.Services;

public sealed class RemoveService
{
    private readonly IHandler handler;

    public RemoveService(IHandler handler)
    {
        this.handler = handler;
    }

    public OperationResult RemoveVehicle(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return OperationResult.Failed(
                "Enter a registration number.",
                OperationError.ValidationFailed);
        }

        return handler.RemoveVehicle(registrationNumber.Trim());
    }
}
