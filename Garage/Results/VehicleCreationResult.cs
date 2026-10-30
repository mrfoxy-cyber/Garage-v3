using Garage.Vehicles;

namespace Garage.Results;

public sealed record VehicleCreationResult
{
    private VehicleCreationResult(Vehicle? vehicle, string message, OperationError error, ParkingResult? parking = null)
    {
        Vehicle = vehicle;
        Message = message;
        Error = error;
        Parking = parking;
    }

    public bool Success => Error == OperationError.None;
    public bool Cancelled => Error == OperationError.Cancelled;
    public Vehicle? Vehicle { get; }
    public string Message { get; }
    public OperationError Error { get; }
    public ParkingResult? Parking { get; }

    public static VehicleCreationResult Completed(Vehicle vehicle, ParkingResult? parking = null, string? message = null)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (parking is { Success: false })
            throw new ArgumentException("A successful creation cannot contain failed parking.", nameof(parking));
        return new(vehicle, message ??
            $"Created {vehicle.GetType().Name}: {vehicle.Manufacturer} {vehicle.Model} ({vehicle.RegistrationNumber}).",
            OperationError.None, parking);
    }

    public static VehicleCreationResult Failed(string message, OperationError error = OperationError.ValidationFailed)
    {
        if (error == OperationError.None)
            throw new ArgumentException("A failed creation needs an error code.", nameof(error));
        return new(null, message, error);
    }

    public static VehicleCreationResult Cancel() =>
        new(null, "Vehicle creation was cancelled.", OperationError.Cancelled);
}
