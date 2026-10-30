using VehicleGarage = Garage.Models.Garage;

namespace Garage.Results;

public sealed record GarageCreationResult
{
    private GarageCreationResult(VehicleGarage? garage, string message, OperationError error)
    {
        Garage = garage;
        Message = message;
        Error = error;
    }

    public bool Success => Error == OperationError.None;
    public VehicleGarage? Garage { get; }
    public string Message { get; }
    public OperationError Error { get; }

    public static GarageCreationResult Completed(VehicleGarage garage)
    {
        ArgumentNullException.ThrowIfNull(garage);
        return new(garage,
            $"Created {garage.GarageName} with {garage.Capacity} spots and {garage.OccupiedSpaces} parked vehicles.",
            OperationError.None);
    }

    public static GarageCreationResult Failed(string message, OperationError error = OperationError.ValidationFailed)
    {
        if (error == OperationError.None)
            throw new ArgumentException("A failed creation needs an error code.", nameof(error));
        return new(null, message, error);
    }
}
